# Project Rules & Conventions

> Tài liệu quy tắc chung cho toàn bộ dự án (BE + FE). Mọi thành viên phải tuân thủ để đảm bảo tính nhất quán.

---

## 1. Tổng quan kiến trúc

### 1.1 Backend - Clean Architecture (.NET 9)

```
ProjectCore.sln
├── ProjectCore.Domain              # Entities, Value Objects, Interfaces, Exceptions
├── ProjectCore.Application         # Use Cases (Commands/Queries), DTOs, Mappers
├── ProjectCore.Infrastructure      # EF Core, Repositories, External Services
└── ProjectCore.Presentation.API    # Controllers, Middleware, JWT, Request/Response Models
```

**Quy tắc phụ thuộc (Dependency Rule):**

```
Domain ← Application ← Infrastructure
                     ← Presentation.API
```

- **Domain** không phụ thuộc bất kỳ layer nào.
- **Application** chỉ phụ thuộc Domain.
- **Infrastructure** phụ thuộc Domain + Application.
- **Presentation.API** phụ thuộc Application + Infrastructure.

### 1.2 Frontend - Next.js App Router (React 19 + TypeScript)

```
fe_nextjs/
├── app/                # Pages & Layouts (App Router)
│   ├── (auth)/         # Public routes (login)
│   └── (dashboard)/    # Protected routes (users, roles)
├── components/         # Reusable UI components
│   ├── ui/             # Atomic components (Button, Input, Loading, Pagination)
│   └── layout/         # Layout components (Sidebar, Header)
├── hooks/              # Custom React hooks
├── lib/                # Utilities & configurations (Axios instance, helpers)
├── services/           # API service layer (auth, user, role)
├── stores/             # Zustand state management
└── types/              # TypeScript interfaces & types
```

---

## 2. Cấu trúc folder chi tiết

### 2.1 BE - Domain Layer

```
ProjectCore.Domain/
├── Entities/               # Domain entities (User, Role, Permission, UserRole, RolePermission)
├── ValueObjects/           # Immutable value objects (Email, UserName, PermissionCode,...)
│   ├── User/               # Value objects thuộc về User (Email, UserName, FullName,...)
│   ├── Role/               # Value objects thuộc về Role (RoleName)
│   └── Permission/         # Value objects thuộc về Permission (PermissionCode)
├── Interfaces/             # Repository interfaces + Search models
│   ├── UserRepository/     # IUserRepository + UserSearch
│   ├── RoleRepository/     # IRoleRepository + RoleSearch
│   └── PermissionRepository/
├── Enums/                  # Domain enums (EntityStatus, Gender)
└── Exceptions/             # Domain-specific exceptions
```

### 2.2 BE - Application Layer

```
ProjectCore.Application/
├── UseCases/               # Tổ chức theo Entity → Action → Command/Query
│   ├── Users/
│   │   ├── Commands/       # Create, Update, Delete, Login
│   │   │   └── CreateUser/
│   │   │       ├── CreateUserCommand.cs
│   │   │       └── CreateUserHandler.cs
│   │   └── Queries/        # GetById, GetAll, GetData, GetByUserNameOrEmail
│   │       └── GetUserById/
│   │           ├── GetUserByIdQuery.cs
│   │           └── GetUserByIdHandler.cs
│   ├── Roles/
│   │   ├── Commands/
│   │   └── Queries/
│   ├── Permissions/
│   └── SeedData/
├── Dtos/                   # Data Transfer Objects
├── Mappings/               # Entity ↔ DTO mappers
├── Common/                 # Shared: Configuration, Security, Models
│   ├── Configuration/      # IAdminSeedConfig, IAdminRoleSeedConfig
│   ├── Models/             # PagedResult<T>
│   └── Security/           # IPasswordHasher
├── Interfaces/             # Cross-cutting interfaces (IUnitOfWork, IPermissionQueryRepository)
└── DependencyInjection.cs
```

### 2.3 BE - Infrastructure Layer

```
ProjectCore.Infrastructure/
├── Persistence/            # EF Core DbContext, EntityConfigurations, UnitOfWork
├── Repositories/           # Repository implementations
├── Configuration/          # AdminSeedConfig, AdminRoleSeedConfig
├── Security/               # BCryptPasswordHasher
├── Migrations/             # EF Core migrations
└── DependencyInjection.cs
```

### 2.4 BE - Presentation.API Layer

```
ProjectCore.Presentation.API/
├── Controllers/            # API controllers
├── Models/
│   ├── Requests/           # Request DTOs (CreateUserRequest, LoginRequest,...)
│   └── Responses/          # ApiResponse<T> wrapper
├── Authentication/         # JWT services (IJwtTokenService, JwtSettings)
├── Middleware/             # ExceptionMiddleware
├── Permissions/            # ApiPermissionScanner
└── Program.cs
```

### 2.5 FE - Page Structure (Next.js App Router)

```
app/
├── (auth)/                 # Route group: public
│   └── login/page.tsx
├── (dashboard)/            # Route group: protected (auth required)
│   ├── layout.tsx          # Sidebar + auth check
│   ├── page.tsx            # Dashboard overview
│   ├── users/
│   │   ├── page.tsx        # Danh sách users
│   │   ├── create/page.tsx # Tạo user
│   │   └── [id]/edit/page.tsx  # Sửa user
│   └── roles/
│       ├── page.tsx
│       ├── create/page.tsx
│       └── [id]/edit/page.tsx
├── layout.tsx              # Root layout
└── page.tsx                # Redirect → /login
```

---

## 3. Quy tắc đặt tên (Naming Conventions)

### 3.1 Backend (C#)

| Loại | Quy tắc | Ví dụ |
|------|---------|-------|
| Project | `ProjectCore.[Layer]` | `ProjectCore.Domain` |
| Entity | PascalCase, số ít | `User`, `Role`, `Permission` |
| Value Object | `sealed record`, PascalCase | `Email`, `UserName`, `PermissionCode` |
| Interface | `I` + PascalCase | `IUserRepository`, `IUnitOfWork` |
| Repository | PascalCase | `UserRepository` |
| Handler | `[Action][Entity]Handler` | `CreateUserHandler`, `GetUserByIdHandler` |
| Command | `[Action][Entity]Command` | `CreateUserCommand`, `LoginUserCommand` |
| Query | `[Action][Entity]Query` | `GetUserByIdQuery` |
| DTO | PascalCase + `Dto` | `UserDto`, `RoleDto` |
| Request model | `[Action][Entity]Request` | `CreateUserRequest`, `LoginRequest` |
| Exception | PascalCase + `Exception` | `UserNotFoundException` |
| Mapper | `[Entity]Mapper` | `UserMapper`, `RoleMapper` |
| Enum | PascalCase | `EntityStatus`, `Gender` |
| Async method | PascalCase + `Async` | `GetByIdAsync()`, `AddAsync()` |
| Private field | `_camelCase` | `_userRepository`, `_logger` |
| Parameter | camelCase | `userId`, `cancellationToken` |

### 3.2 Frontend (TypeScript/React)

| Loại | Quy tắc | Ví dụ |
|------|---------|-------|
| Component | PascalCase `.tsx` | `Button.tsx`, `Sidebar.tsx` |
| Page | `page.tsx` (Next.js convention) | `app/(dashboard)/users/page.tsx` |
| Layout | `layout.tsx` | `app/(dashboard)/layout.tsx` |
| Service | `camelCase.service.ts` | `auth.service.ts`, `user.service.ts` |
| Store | `kebab-case-store.ts` | `auth-store.ts` |
| Type file | `camelCase.ts` trong `/types` | `user.ts`, `api.ts` |
| Hook | `use[Name].ts` | `useAuth.ts`, `useDebounce.ts` |
| Utility | `camelCase.ts` trong `/lib` | `utils.ts`, `api.ts` |
| Interface/Type | PascalCase | `User`, `ApiResponse<T>`, `LoginRequest` |
| Function | camelCase | `formatDate()`, `getInitials()` |
| Component props | `[Component]Props` | `ButtonProps`, `InputProps` |
| Biến, tham số | camelCase | `accessToken`, `isAuthenticated` |

---

## 4. Chuẩn API Response (BE → FE)

### 4.1 Response wrapper chung

Mọi API endpoint **bắt buộc** trả về qua `ApiResponse<T>`:

```csharp
// BE - C#
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; }
    public List<string> Errors { get; set; }
}
```

```typescript
// FE - TypeScript
interface ApiResponse<T = unknown> {
  success: boolean;
  data: T;
  message: string;
  errors: string[];
}
```

### 4.2 Response theo loại thao tác

| Thao tác | HTTP Method | Status Code | Data |
|----------|-------------|-------------|------|
| Lấy danh sách | GET | 200 | `PagedResult<T>` hoặc `T[]` |
| Lấy chi tiết | GET | 200 / 404 | `T` hoặc error message |
| Tạo mới | POST | 201 (CreatedAtAction) | `{ id }` |
| Cập nhật | PUT | 200 | Success message |
| Xóa | DELETE | 200 | Success message |
| Login | POST | 200 / 401 | `{ accessToken, refreshToken, user }` |

### 4.3 Pagination format

```csharp
// BE - C#
public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}
```

```typescript
// FE - TypeScript
interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageIndex: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
```

### 4.4 Ví dụ response thực tế

**Thành công (lấy danh sách):**
```json
{
  "success": true,
  "data": {
    "items": [{ "id": "...", "userName": "admin" }],
    "totalCount": 50,
    "pageIndex": 1,
    "pageSize": 10,
    "totalPages": 5,
    "hasPreviousPage": false,
    "hasNextPage": true
  },
  "message": "Success",
  "errors": []
}
```

**Thành công (tạo mới):**
```json
{
  "success": true,
  "data": { "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6" },
  "message": "User created successfully",
  "errors": []
}
```

**Lỗi:**
```json
{
  "success": false,
  "data": null,
  "message": "Email already exists",
  "errors": ["Email 'test@test.com' is already in use"]
}
```

### 4.5 JSON serialization

- BE serialize bằng `System.Text.Json` với `camelCase` policy.
- FE nhận trực tiếp, property name khớp 1:1.

---

## 5. Error Handling

### 5.1 BE - Exception hierarchy

```
Exception
└── DomainException (abstract)
    ├── UserNotFoundException
    ├── UserEmailAlreadyExistsException
    ├── UserNameAlreadyExistsException
    ├── UserAlreadyHasRoleException
    ├── UserDoesNotHaveRoleException
    ├── UserMustHaveAtLeastOneRoleException
    ├── InvalidLoginException
    ├── RoleNotFoundException
    ├── RoleNameAlreadyExistsException
    ├── RoleInUseException
    └── CannotDeleteAdminRoleException
```

### 5.2 BE - Exception → HTTP Status Code mapping (ExceptionMiddleware)

| Exception Type | HTTP Status | Message |
|----------------|-------------|---------|
| `UnauthorizedAccessException` | 401 | "Unauthorized" |
| `KeyNotFoundException` | 404 | "Resource not found" |
| `ArgumentException` | 400 | Exception message |
| `InvalidOperationException` | 400 | Exception message |
| Mọi exception khác | 500 | "An unexpected error occurred" |

### 5.3 FE - Error handling pattern

```typescript
// Service layer: throw cho component xử lý
const response = await userService.create(data);

// Component layer: try-catch + toast
try {
  const res = await userService.create(data);
  if (res.success) {
    toast.success(res.message);
    router.push("/users");
  } else {
    toast.error(res.message);
  }
} catch (error) {
  toast.error("An unexpected error occurred");
}
```

---

## 6. Authentication & Authorization

### 6.1 Luồng Login

```
FE: POST /api/auth/login { userNameOrEmail, password }
        ↓
BE: Validate → Hash check → Generate JWT + Refresh Token
        ↓
FE: Nhận { accessToken, refreshToken, user: { id, userName, email, permissions[] } }
        ↓
FE: Lưu vào Zustand store (persist → localStorage key: "auth-storage")
```

### 6.2 JWT Token Structure

```
Header:  { alg: "HS256", typ: "JWT" }
Payload: {
  nameid:     userId (Guid),
  unique_name: userName,
  email:      email,
  permission: ["USERS.CREATE", "ROLES.UPDATE", ...],  // multiple claims
  exp:        expiration (15 minutes),
  iss:        "ProjectCore.API",
  aud:        "ProjectCore.Client"
}
```

### 6.3 Token Refresh Flow

```
FE: Request fails with 401
        ↓
FE: Axios interceptor queues request, calls POST /api/auth/refresh
        ↓
BE: Validate old refresh token → Generate new tokens
        ↓
FE: Update store → Retry original request
        ↓
Nếu refresh cũng fail → Logout + redirect /login
```

### 6.4 Permission format

- BE tạo: `"{Module}.{Action}"` uppercase → ví dụ `"USERS.CREATE"`, `"ROLES.DELETE"`
- FE kiểm tra: `authStore.hasPermission("USERS.CREATE")`

### 6.5 Route Protection

- **BE:** `[Authorize]` attribute trên controller/action.
- **FE:** Dashboard layout kiểm tra `isAuthenticated` từ Zustand store → redirect `/login` nếu false.

---

## 7. Quy tắc chung

### 7.1 BE - Domain Entity

- Mọi entity kế thừa `DomainEntity<TKey>`.
- Audit fields tự động: `CreatedDate`, `CreatedBy`, `UpdatedDate`, `UpdatedBy`.
- `EntityStatus` (Active/InActive) cho soft delete.
- DateTime luôn dùng **UTC**.
- Constructor `protected` cho EF Core, public constructor yêu cầu đủ tham số bắt buộc.

### 7.2 BE - Value Object

- Luôn là `sealed record`.
- Validate trong constructor, throw `ArgumentException` nếu invalid.
- Property chính là `Value` (kiểu string).
- Không có public parameterless constructor.

### 7.3 BE - Repository

- Interface đặt trong **Domain layer**.
- Implementation đặt trong **Infrastructure layer**.
- `AddAsync()` / `Update()` / `Remove()` chỉ thao tác change tracker, không gọi SaveChanges.
- SaveChanges/Commit nằm ở **UnitOfWork**.
- Mọi async method nhận `CancellationToken`.
- Query read-only dùng `AsNoTracking()`.
- Quan hệ navigation dùng `.Include()` khi cần.

### 7.4 BE - Use Case (Handler)

- Mỗi use case = 1 folder riêng chứa Command/Query + Handler.
- Pattern: `UseCases/{Entity}/{Commands|Queries}/{ActionName}/`.
- Handler **không** gọi trực tiếp SaveChanges - để UnitOfWork quản lý.
- Handler nhận dependencies qua constructor injection.

### 7.5 BE - Controller

- Route: `[Route("api/[controller]")]` → `/api/users`, `/api/roles`.
- Mọi response qua `ApiResponse<T>.Ok()` hoặc `ApiResponse.Fail()`.
- Không chứa business logic - chỉ map request → command/query → gọi handler → trả response.
- `[Authorize]` trên controller level (trừ login endpoint).

### 7.6 BE - DI Registration

- Repository, Handler, Service: `AddScoped<>`.
- Configuration: `AddSingleton<>`.
- Mỗi layer có `DependencyInjection.cs` riêng (extension method trên `IServiceCollection`).

### 7.7 FE - Component

- Atomic UI components trong `/components/ui/` - không chứa business logic.
- Layout components trong `/components/layout/`.
- Style bằng **Tailwind CSS** utility classes, không dùng CSS modules.
- Props interface đặt cùng file component.

### 7.8 FE - Service Layer

- Mỗi entity 1 service file: `{entity}.service.ts`.
- Service gọi API qua Axios instance từ `lib/api.ts`.
- Trả về typed `ApiResponse<T>`.
- Không chứa state management logic.

### 7.9 FE - State Management

- Dùng **Zustand** với persist middleware.
- Store file: `stores/{name}-store.ts`.
- Chỉ persist data cần thiết (tokens, user info).
- `getState()` cho non-React context (interceptor), `useStore()` hook cho components.

### 7.10 FE - Form Validation

- Dùng **Zod** schema + **React Hook Form**.
- Schema định nghĩa tại page level, cùng file page component.
- Error hiển thị qua `<Input error={...} />`.

### 7.11 FE - Type Definitions

- Mỗi entity 1 file type trong `/types/`.
- Type phải khớp 1:1 với BE response (camelCase).
- Generic types chung (`ApiResponse<T>`, `PagedResult<T>`) trong `types/api.ts`.

---

## 8. Quy tắc đồng bộ BE ↔ FE

### 8.1 Khi thêm entity/feature mới

| Bước | BE | FE |
|------|----|----|
| 1 | Tạo Entity + Value Objects trong Domain | Tạo type trong `types/{entity}.ts` |
| 2 | Tạo Repository interface trong Domain | - |
| 3 | Tạo DTOs, Mappers trong Application | Đảm bảo type FE khớp với DTO BE |
| 4 | Tạo Commands/Queries + Handlers | - |
| 5 | Implement Repository trong Infrastructure | - |
| 6 | Đăng ký DI trong DependencyInjection.cs | - |
| 7 | Tạo Request models + Controller | Tạo service trong `services/{entity}.service.ts` |
| 8 | - | Tạo pages trong `app/(dashboard)/{entity}/` |
| 9 | - | Tạo components nếu cần |

### 8.2 Checklist đồng bộ type

Khi BE thay đổi response format:

- [ ] Cập nhật DTO/Response model ở BE
- [ ] Cập nhật type tương ứng ở FE (`types/*.ts`)
- [ ] Cập nhật service nếu endpoint thay đổi (`services/*.service.ts`)
- [ ] Kiểm tra tất cả component đang sử dụng type đó

### 8.3 Quy tắc property name

- BE serialize: `camelCase` (System.Text.Json).
- FE interface: `camelCase`.
- **Phải khớp nhau 100%.**

Ví dụ:
```csharp
// BE DTO
public class UserDto {
    public string UserName { get; set; }     // → JSON: "userName"
    public string Email { get; set; }         // → JSON: "email"
    public List<Guid> RoleIds { get; set; }   // → JSON: "roleIds"
}
```
```typescript
// FE Type
interface User {
  userName: string;     // khớp "userName"
  email: string;        // khớp "email"
  roleIds: string[];    // khớp "roleIds"
}
```

---

## 9. API Endpoint Conventions

### 9.1 URL format

```
GET    /api/{entity}              # Danh sách (có pagination)
GET    /api/{entity}/{id}         # Chi tiết
POST   /api/{entity}              # Tạo mới
PUT    /api/{entity}/{id}         # Cập nhật
DELETE /api/{entity}/{id}         # Xóa
```

### 9.2 Query parameters cho danh sách

| Param | Kiểu | Mô tả |
|-------|------|-------|
| `page` | int | Trang hiện tại (default: 1) |
| `pageSize` | int | Số item/trang (default: 10) |
| `keyword` | string | Tìm kiếm chung |
| `sortBy` | string | Tên field sort (PascalCase: "UserName", "Email") |
| `sortDescending` | bool | Thứ tự giảm dần |
| Các filter riêng | string | Filter theo field cụ thể |

### 9.3 Ví dụ

```
GET /api/users?page=1&pageSize=10&keyword=admin&sortBy=UserName&sortDescending=false
GET /api/roles?page=1&pageSize=10&name=ADMIN
```

---

## 10. Logging (Serilog)

### 10.1 Cấu hình

- **Console**: format `[HH:mm:ss LVL] Message`
- **File**: `Logs/log-{Date}.txt`, rolling daily
- MinimumLevel: `Information` (default), `Warning` cho Microsoft/EF Core

### 10.2 Quy tắc log

| Level | Khi nào dùng |
|-------|-------------|
| `Information` | Bắt đầu/kết thúc operation, business event quan trọng |
| `Warning` | Tình huống bất thường nhưng app vẫn chạy |
| `Error` | Exception được catch và xử lý |
| `Fatal` | App không thể tiếp tục (startup failure) |
| `Debug` | Chi tiết debug (chỉ bật khi cần) |

### 10.3 Structured logging format

```csharp
// Dùng template, KHÔNG string interpolation
_logger.LogInformation("Created user {UserName} (Id={UserId})", userName, userId);  // GOOD
_logger.LogInformation($"Created user {userName}");                                  // BAD
```

---

## 11. Tech Stack Summary

### Backend
| Thành phần | Công nghệ |
|-----------|-----------|
| Framework | ASP.NET Core 9 |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Auth | JWT Bearer + Refresh Token |
| Password | BCrypt.Net |
| Logging | Serilog |
| Env | DotNetEnv (.env support) |

### Frontend
| Thành phần | Công nghệ |
|-----------|-----------|
| Framework | Next.js 16 (App Router) |
| Language | TypeScript 5 |
| UI | React 19 |
| Styling | Tailwind CSS 4 |
| HTTP Client | Axios |
| State | Zustand (persist) |
| Forms | React Hook Form + Zod |
| Icons | Lucide React |
| Toast | React Hot Toast |
