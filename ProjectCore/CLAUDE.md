# CLAUDE.md — ProjectCore

Tài liệu này là **bộ quy tắc chính** cho Claude Code khi làm việc với solution `ProjectCore`.
Đọc file này trước khi thực hiện bất kỳ thay đổi nào trong codebase.

---

## 1. Tổng quan dự án

| Mục | Giá trị |
|-----|---------|
| **Tên solution** | `ProjectCore` |
| **Mục đích** | User & Role Management API — nền tảng quản lý người dùng, phân quyền theo role/permission |
| **Kiến trúc** | Clean Architecture + CQRS (MediatR) + DDD tactical patterns |
| **Framework** | ASP.NET Core 9 (Presentation), .NET 8 (Domain/Application/Infrastructure) |
| **Database** | SQL Server (EF Core 9) |
| **Auth** | JWT Bearer (HS256) + Refresh Token |
| **Frontend** | Next.js (localhost:3000) |

---

## 2. Cấu trúc solution

```
ProjectCore/
├── ProjectCore.Domain/              ← Layer 1: Business rules, Entities, VOs, Exceptions
├── ProjectCore.Application/         ← Layer 2: Use cases (CQRS handlers), DTOs, Interfaces
├── ProjectCore.Infrastructure/      ← Layer 3: EF Core, Repositories, Security, UoW
└── ProjectCore.Presentation.API/    ← Layer 4: Controllers, Middleware, JWT, Models
```

### Thứ tự project reference (KHÔNG được đảo ngược)

```
Domain  ←  Application  ←  Infrastructure
                        ←  Presentation.API  →  Infrastructure
```

- **Domain**: không reference project nào trong solution
- **Application**: chỉ reference `Domain`
- **Infrastructure**: reference `Domain` + `Application` (implement interface từ cả hai)
- **Presentation.API**: reference `Application` + `Infrastructure` (composition root)

---

## 3. Quy tắc Dependency (TUYỆT ĐỐI không vi phạm)

| Quy tắc | Chi tiết |
|---------|----------|
| Domain không biết Infrastructure | Không `using ProjectCore.Infrastructure` trong Domain |
| Application không biết Infrastructure | Handler chỉ inject interface, không inject `DbContext`, `Repository` concrete |
| Controller không gọi Repository trực tiếp | Luồng nghiệp vụ đi qua Application layer (MediatR.Send) |
| Infrastructure implement interface của Domain/Application | `IUserRepository` ở Domain → `UserRepository` ở Infrastructure |

> **Ngoại lệ chấp nhận:** `Infrastructure` reference `Application` là OK vì một số interface (`IUnitOfWork`, `IPermissionQueryRepository`, `IPasswordHasher`) đặt ở Application.

---

## 4. CQRS Pattern (MediatR)

Mọi use case **phải** triển khai pattern sau:

```
Command/Query (IRequest<TResponse>)  ←→  Handler (IRequestHandler<TRequest, TResponse>)
```

### Quy tắc bắt buộc

- **Command**: thay đổi state → `IRequest<Guid>` (create) hoặc `IRequest<Unit>` (update/delete)
- **Query**: đọc dữ liệu → `IRequest<TDto>` hoặc `IRequest<PagedResult<TDto>>`
- **Handler**: `sealed class`, inject interface (không inject class concrete)
- **Dispatch**: luôn qua `IMediator.Send(command, cancellationToken)` — không gọi handler trực tiếp
- **Pipeline**: `LoggingBehavior<,>` đã đăng ký — tự động log start/end/duration/error

### File structure cho mỗi use case

```
UseCases/{Domain}/{Commands|Queries}/{UseCaseName}/
    ├── {UseCaseName}Command.cs    (hoặc Query.cs)
    └── {UseCaseName}Handler.cs
```

---

## 5. Domain Layer — Quy tắc

### Entities

- `DomainEntity<TKey>` là base class — **không** thêm public setter trên property domain
- Thay đổi state qua method có tên rõ ý nghĩa: `UpdateProfile()`, `AssignRole()`, `UpdateDetails()`
- ID luôn là `Guid` — tạo `Guid.NewGuid()` ở Application layer khi gọi constructor entity

### Value Objects

| Value Object | Validation |
|-------------|-----------|
| `UserName` | 3–50 ký tự, trimmed |
| `Email` | Valid email format, lowercase |
| `FullName` | Non-empty string |
| `PhoneNumber` | Regex: `^(0\|+84)[0-9]{9}$` |
| `Gender` | Enum: Unknown, Male, Female, Other |
| `Avatar` | Valid URI hoặc file path |
| `RoleName` | Max 50 chars, non-empty |
| `PermissionCode` | Format: `MODULE.ACTION` (uppercase) |

- Value Object **immutable**: chỉ set lúc khởi tạo, validate trong constructor
- Value Object **không cho phép null** thuộc tính cốt lõi — throw `DomainException` nếu invalid

### Exceptions

Domain exceptions đặt trong `ProjectCore.Domain/Exceptions/`:

| Exception | HTTP Status | Mô tả |
|-----------|------------|-------|
| `UserNotFoundException` | 404 | User không tồn tại |
| `UserEmailAlreadyExistsException` | 409 | Email đã tồn tại |
| `UserNameAlreadyExistsException` | 409 | Username đã tồn tại |
| `InvalidLoginException` | 401 | Sai thông tin đăng nhập |
| `RoleNotFoundException` | 404 | Role không tồn tại |
| `RoleNameAlreadyExistsException` | 409 | Tên role đã tồn tại |
| `RoleInUseException` | 400 | Role đang được gán cho user |
| `CannotDeleteAdminRoleException` | 400 | Không xóa được role ADMIN |

**Không throw `Exception` generic** — luôn dùng exception domain cụ thể.

---

## 6. Application Layer — Quy tắc

### DependencyInjection

- File: `DependencyInjection.cs`, class `DependencyInjection`, method `AddApplication()`
- Đăng ký MediatR + `LoggingBehavior`
- **TODO:** đổi tên theo convention: `ApplicationLayerExtension.cs` / `AddApplicationLayer()`

### Interfaces (port) đặt ở Application

- `IUnitOfWork` — `SaveChangesAsync`, `BeginTransactionAsync`, `CommitAsync`, `RollbackAsync`
- `IPermissionQueryRepository` — `GetPermissionsByUserIdAsync(userId)`
- `IPasswordHasher` — `Hash(password)`, `Verify(hashedPassword, plainPassword)`
- `IPermissionScanner` — `Scan()` → `IEnumerable<PermissionScanResult>` (**TODO:** nên chuyển sang Presentation layer)

### DTOs

- `UserDto` và `RoleDto` kế thừa `DomainDto` (Status, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
- **Không** trả Entity ra ngoài Application layer — luôn map sang DTO
- Mapper: static class `UserMapper.ToDto()`, `RoleMapper.ToDto()`

### PagedResult\<T\>

```csharp
new PagedResult<T>(items, totalCount, pageIndex, pageSize)
// Properties: Items, TotalCount, PageIndex, PageSize, TotalPages, HasPreviousPage, HasNextPage
```

---

## 7. Infrastructure Layer — Quy tắc

### DependencyInjection

- File: `DependencyInjection.cs`, method `AddInfrastructure(IConfiguration)`
- **TODO:** đổi tên: `InfrastructureLayerExtension.cs` / `AddInfrastructureLayer(config)`

### Repository pattern

- Interface ở Domain (`IUserRepository`, `IRoleRepository`, `IPermissionRepository`)
- Implementation ở Infrastructure (`Repositories/`)
- Repository **không** trả `IQueryable` ra ngoài
- Pagination: method `GetDataAsync(SearchBase, CancellationToken)` → `(IEnumerable<T>, int totalCount)`

### EF Core

- `ApplicationDbContext`: DbSet cho User, UserRole, Role, Permission, RolePermission
- Entity configurations: `Persistence/Configurations/` (Fluent API)
- Migrations: `Persistence/Migrations/`
- **Không** dùng raw SQL ngoại trừ `PermissionQueryRepository` (LINQ join phức tạp)

### Security

- Password hashing: `BCryptPasswordHasher` (BCrypt.Net-Next)
- **Không** lưu plain-text password ở bất kỳ đâu

---

## 8. Presentation.API Layer — Quy tắc

### Controllers

- Kế thừa `ControllerBase`, attribute `[ApiController]`, `[Route("api/[controller]")]`
- Hầu hết controller `[Authorize]` — Auth endpoints không có (anonymous)
- Controller chỉ làm 2 việc: nhận request → map sang Command/Query → dispatch qua `IMediator.Send()`
- **Không** nhồi business logic trong controller
- Đọc `CurrentUserId` từ JWT claims: `Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)`

### Response format

Mọi API trả về `ApiResponse<T>` hoặc `ApiResponse`:

```json
{
  "success": true|false,
  "data": { ... },
  "message": "...",
  "errors": []
}
```

### Middleware

- `ExceptionMiddleware`: bắt domain exceptions → map HTTP status code
- `SerilogRequestLogging`: log mọi request/response
- Thứ tự: `SerilogRequestLogging` → `ExceptionMiddleware` → `UseHttpsRedirection` → `UseCors` → `UseAuthentication` → `UseAuthorization` → `MapControllers`

### JWT

- Access token: HS256, claims = `NameIdentifier(userId)`, `Name(userName)`, `Email`, `permission` (multi-value)
- Refresh token: 64-byte random base64, TTL = 7 ngày
- **CRITICAL TODO:** Refresh token đang lưu `static ConcurrentDictionary` — phải migrate sang DB/Redis trước production

---

## 9. Naming Conventions

| Thành phần | Quy tắc | Ví dụ |
|------------|---------|--------|
| Namespace | PascalCase | `ProjectCore.Application.UseCases.Users` |
| Class | PascalCase | `CreateUserHandler`, `UserRepository` |
| Interface | `I` + PascalCase | `IUserRepository`, `IUnitOfWork` |
| Method | PascalCase + động từ | `GetByIdAsync()`, `SaveChangesAsync()` |
| Private field | `_` + camelCase | `_userRepository`, `_unitOfWork` |
| Async method | hậu tố `Async` | `HandleAsync`, `GetAllAsync` |
| Command | `{UseCase}Command` | `CreateUserCommand` |
| Query | `{UseCase}Query` | `GetUserByIdQuery` |
| Handler | `{UseCase}Handler` | `CreateUserHandler` |
| DTO | `{Entity}Dto` | `UserDto`, `RoleDto` |
| Request model | `{UseCase}Request` | `CreateUserRequest` |

---

## 10. Coding Conventions (C# / .NET)

- **Nullable**: `#nullable enable` cấp project (đã bật). Dùng `ArgumentNullException.ThrowIfNull(x)` tại entry public API
- **Async**: mọi method async công khai nhận `CancellationToken` và truyền xuống EF/repo
- **Async void**: không dùng ngoại trừ event handler
- **ImplicitUsings**: đã bật — **không** thêm `using System;`, `using System.Collections.Generic;`, etc. thừa
- **File-scoped namespace**: ưu tiên dùng `namespace X;` (giảm indent)
- **sealed**: Handler, Command, Query nên là `sealed class`
- **Record**: dùng cho DTO nhỏ, response models, value mang dữ liệu thuần
- **String interpolation logging**: `_logger.LogInformation("User {UserId} created", userId)` — không nối chuỗi

---

## 11. Các vấn đề đã biết (Known Issues / TODOs)

| # | Mức độ | Vị trí | Vấn đề |
|---|--------|--------|---------|
| 1 | 🔴 Critical | `GetAllUsersHandler` | Không implement `IRequestHandler<,>` — không dispatch được qua MediatR |
| 2 | 🔴 Critical | `AuthController._refreshTokens` | `static ConcurrentDictionary` — mất token khi restart, không scale ngang |
| 3 | 🟠 High | `IPermissionScanner` | Đặt ở Application nhưng là Presentation concern — vi phạm layer boundary |
| 4 | 🟠 High | `Program.cs` SeedDataHandler | `GetRequiredService<SeedDataHandler>()` cần có DI registration rõ ràng |
| 5 | 🟡 Medium | DI naming | `DependencyInjection.cs` → `*LayerExtension.cs`, `AddApplication()` → `AddApplicationLayer()` |
| 6 | 🟡 Medium | TFM | Domain/Application/Infrastructure target `net8.0`, Presentation.API target `net9.0` — nên đồng nhất |
| 7 | 🟡 Medium | `UserNotFoundException` | Không truyền userId vào message — không nhất quán với `RoleNotFoundException(id)` |
| 8 | 🟡 Medium | DI order | `Program.cs` gọi `AddInfrastructure` trước `AddApplication` — nên đảo lại |
| 9 | 🟢 Low | Unused file | `UpdateUserProfileHandler.cs` — deprecated placeholder, có thể xóa |
| 10 | 🟢 Low | Thừa `using` | `IPermissionScanner.cs`, `GetAllUsersHandler.cs`, các DTO files — có ImplicitUsings nhưng vẫn còn using thủ công |

---

## 12. Quy trình thêm Use Case mới

1. **Domain** (nếu cần): thêm method vào Entity hoặc interface Repository
2. **Application**: tạo folder `UseCases/{Domain}/{Commands|Queries}/{UseCaseName}/`
   - `{UseCase}Command.cs` (hoặc `Query.cs`) implement `IRequest<TResponse>`
   - `{UseCase}Handler.cs` implement `IRequestHandler<TCommand, TResponse>`
3. **Infrastructure** (nếu cần): thêm method vào Repository implementation
4. **Presentation.API**: thêm action vào Controller, tạo Request model nếu cần, map sang Command/Query
5. **Không** cần đăng ký thủ công — MediatR auto-scan assembly

---

## 13. Environment Variables

| Biến | Bắt buộc | Mô tả |
|------|----------|-------|
| `ADMIN_USERNAME` | ✅ | Username của admin seed |
| `ADMIN_EMAIL` | ✅ | Email của admin seed |
| `ADMIN_PASSWORD` | ✅ | Password của admin seed |
| `ADMIN_FULLNAME` | ❌ | Full name của admin seed |
| `ADMIN_ROLE_NAME` | ❌ | Tên role admin (default: `ADMIN`) |
| `ADMIN_ROLE_DESCRIPTION` | ❌ | Mô tả role admin |

Có thể đặt trong file `.env` ở root project (DotNetEnv load lúc startup).

---

## 14. appsettings.json cần thiết

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=ProjectCore;..."
  },
  "JwtSettings": {
    "SecretKey": "...",
    "Issuer": "ProjectCore",
    "Audience": "ProjectCore",
    "AccessTokenExpirationMinutes": 60
  },
  "AdminSeed": {
    "UserName": "admin",
    "Email": "admin@example.com",
    "Password": "Admin@123",
    "FullName": "System Administrator"
  },
  "AdminRole": {
    "Name": "ADMIN",
    "Description": "System Administrator Role"
  }
}
```
