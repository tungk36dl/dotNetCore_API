# API Specification — ProjectCore

**Base URL:** `https://{host}/api`  
**Auth:** JWT Bearer Token (header `Authorization: Bearer {accessToken}`)  
**Content-Type:** `application/json`

## Swagger UI

| Môi trường | URL | Ghi chú |
|------------|-----|---------|
| Development | `http://localhost:5036/swagger` hoặc `https://localhost:7124/swagger` | Bật tự động |
| Production | Không khả dụng | UI bị ẩn; JSON spec vẫn tại `/swagger/v1/swagger.json` cho internal tooling |

---

## Định dạng Response chung

Mọi endpoint đều trả về envelope `ApiResponse<T>`:

```json
{
  "success": true,
  "data": { ... },
  "message": "Success",
  "errors": []
}
```

| Field | Type | Mô tả |
|-------|------|-------|
| `success` | `boolean` | `true` nếu thành công |
| `data` | `T \| null` | Payload (null khi lỗi) |
| `message` | `string` | Thông báo kết quả |
| `errors` | `string[]` | Danh sách lỗi validation hoặc lỗi nghiệp vụ |

---

## HTTP Status Codes

| Code | Ý nghĩa |
|------|---------|
| `200 OK` | Thành công (GET, PUT, DELETE, POST không tạo resource) |
| `201 Created` | Tạo resource thành công (POST create) — kèm `Location` header |
| `400 Bad Request` | Validation lỗi, vi phạm nghiệp vụ |
| `401 Unauthorized` | Chưa đăng nhập hoặc token hết hạn |
| `404 Not Found` | Resource không tồn tại |
| `409 Conflict` | Resource đã tồn tại (email/username/roleName trùng) |
| `422 Unprocessable Entity` | Tham chiếu đến resource không hợp lệ (vd. permissionId không tồn tại) |
| `500 Internal Server Error` | Lỗi server không xác định |

---

## Domain Objects

### UserDto

```json
{
  "id": "guid",
  "userName": "string",
  "email": "string",
  "fullName": "string | null",
  "phoneNumber": "string | null",
  "gender": "Unknown | Male | Female | Other | null",
  "dateOfBirth": "YYYY-MM-DD | null",
  "address": "string | null",
  "avatarUrl": "string | null",
  "roleIds": ["guid", "..."],
  "status": "Active | InActive",
  "createdBy": "guid | null",
  "createdDate": "ISO8601 | null",
  "updatedBy": "guid | null",
  "updatedDate": "ISO8601 | null"
}
```

### RoleDto

```json
{
  "id": "guid",
  "name": "string",
  "description": "string | null",
  "permissionIds": ["guid", "..."],
  "status": "Active | InActive",
  "createdBy": "guid | null",
  "createdDate": "ISO8601 | null",
  "updatedBy": "guid | null",
  "updatedDate": "ISO8601 | null"
}
```

### PagedResult\<T\>

```json
{
  "items": [ /* T[] */ ],
  "totalCount": 100,
  "pageIndex": 1,
  "pageSize": 10,
  "totalPages": 10,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

---

# Auth API

## POST /api/auth/login

Đăng nhập, trả về JWT access token + refresh token.

**Auth:** Không yêu cầu

### Request Body

```json
{
  "userNameOrEmail": "string",   // username hoặc email
  "password": "string"
}
```

### Response `200 OK`

```json
{
  "success": true,
  "data": {
    "accessToken": "eyJ...",
    "refreshToken": "base64string...",
    "user": {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "userName": "admin",
      "email": "admin@example.com",
      "permissions": ["USERS.CREATE", "ROLES.READ", "..."]
    }
  },
  "message": "Login successful",
  "errors": []
}
```

### Errors

| Status | message | Khi nào |
|--------|---------|---------|
| `401` | `"Invalid credentials"` | Sai username/email hoặc password |
| `400` | `"Validation failed"` | Request body thiếu field bắt buộc |

---

## POST /api/auth/refresh

Lấy access token mới bằng refresh token còn hiệu lực.

**Auth:** Không yêu cầu

### Request Body

```json
{
  "accessToken": "eyJ...",       // access token cũ (có thể đã hết hạn)
  "refreshToken": "base64string..."
}
```

### Response `200 OK`

```json
{
  "success": true,
  "data": {
    "accessToken": "eyJ...",
    "refreshToken": "newBase64string..."
  },
  "message": "Token refreshed",
  "errors": []
}
```

### Errors

| Status | message | Khi nào |
|--------|---------|---------|
| `401` | `"Invalid access token"` | Access token không hợp lệ (sai format/signature) |
| `401` | `"Invalid or expired refresh token"` | Refresh token không tồn tại hoặc đã hết hạn (7 ngày) |

> **Lưu ý:** Mỗi lần refresh, cặp token cũ bị hủy và trả về cặp mới. Refresh token được rotate mỗi lần dùng.

---

## GET /api/auth/me

Trả về thông tin user hiện tại từ JWT claims.

**Auth:** Yêu cầu Bearer Token

### Response `200 OK`

```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "userName": "admin",
    "email": "admin@example.com",
    "permissions": ["USERS.CREATE", "ROLES.READ"]
  },
  "message": "Success",
  "errors": []
}
```

### Errors

| Status | Khi nào |
|--------|---------|
| `401` | Token không hợp lệ hoặc hết hạn |

---

## POST /api/auth/logout

Hủy refresh token hiện tại (invalidate).

**Auth:** Yêu cầu Bearer Token

### Request Body

```json
{
  "refreshToken": "base64string..."   // optional — bỏ qua nếu không có
}
```

> **Lưu ý:** `refreshToken` là optional. Nếu không truyền, access token vẫn hết hiệu lực sau TTL nhưng refresh token không bị hủy khỏi store.

### Response `200 OK`

```json
{
  "success": true,
  "data": null,
  "message": "Logged out successfully",
  "errors": []
}
```

---

# Users API

Tất cả endpoint yêu cầu `Authorization: Bearer {token}`.

---

## GET /api/users

Lấy danh sách user có phân trang, tìm kiếm và lọc.

**Auth:** Yêu cầu Bearer Token

### Query Parameters

| Param | Type | Default | Validation | Mô tả |
|-------|------|---------|------------|-------|
| `keyword` | `string?` | - | - | Tìm trong userName, email, fullName |
| `userName` | `string?` | - | - | Lọc chính xác theo userName |
| `email` | `string?` | - | - | Lọc theo email |
| `fullName` | `string?` | - | - | Lọc theo fullName |
| `gender` | `string?` | - | `Unknown \| Male \| Female \| Other` | Lọc theo giới tính |
| `roleId` | `guid?` | - | Valid GUID | Lọc user thuộc role này |
| `sortBy` | `string?` | - | `UserName \| Email \| FullName \| CreatedDate` | Trường sắp xếp |
| `sortDescending` | `boolean` | `false` | - | Sắp xếp giảm dần |
| `page` | `int` | `1` | ≥ 1 | Số trang (bắt đầu từ 1) |
| `pageSize` | `int` | `10` | 1–100 | Số item mỗi trang |

### Response `200 OK`

```json
{
  "success": true,
  "data": {
    "items": [ /* UserDto[] */ ],
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

---

## GET /api/users/{id}

Lấy thông tin user theo ID.

**Auth:** Yêu cầu Bearer Token

### Path Parameters

| Param | Type | Mô tả |
|-------|------|-------|
| `id` | `guid` | User ID |

### Response `200 OK`

```json
{
  "success": true,
  "data": { /* UserDto */ },
  "message": "Success",
  "errors": []
}
```

### Errors

| Status | message | Khi nào |
|--------|---------|---------|
| `404` | `"User not found"` | ID không tồn tại |

---

## GET /api/users/roles

Lấy toàn bộ danh sách role (dùng cho dropdown assign role).

**Auth:** Yêu cầu Bearer Token

### Response `200 OK`

```json
{
  "success": true,
  "data": [ /* RoleDto[] */ ],
  "message": "Success",
  "errors": []
}
```

---

## POST /api/users

Tạo user mới.

**Auth:** Yêu cầu Bearer Token

### Request Body

```json
{
  "userName": "string",    // Required, 3–50 ký tự
  "email": "string",       // Required, valid email format
  "password": "string"     // Required, tối thiểu 6 ký tự
}
```

### Validation Rules

| Field | Ràng buộc |
|-------|-----------|
| `userName` | Required, 3–50 ký tự |
| `email` | Required, đúng format email |
| `password` | Required, tối thiểu 6 ký tự |

### Response `201 Created`

Header: `Location: /api/users/{newUserId}`

```json
{
  "success": true,
  "data": { "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6" },
  "message": "User created successfully",
  "errors": []
}
```

### Errors

| Status | message | Khi nào |
|--------|---------|---------|
| `400` | `"Validation failed"` | Field không hợp lệ |
| `409` | `"Email already exists"` | Email đã được dùng |
| `409` | `"Username already exists"` | Username đã được dùng |

---

## PUT /api/users/{id}

Cập nhật thông tin profile user.

**Auth:** Yêu cầu Bearer Token

### Path Parameters

| Param | Type | Mô tả |
|-------|------|-------|
| `id` | `guid` | User ID |

### Request Body

Tất cả fields đều optional — chỉ gửi những field muốn cập nhật:

```json
{
  "fullName": "string | null",
  "phoneNumber": "string | null",   // Format: 0xxxxxxxxx hoặc +84xxxxxxxxx
  "gender": "Unknown | Male | Female | Other | null",
  "dateOfBirth": "YYYY-MM-DD | null",
  "address": "string | null",
  "avatarUrl": "string | null"      // Valid URL
}
```

### Response `200 OK`

```json
{
  "success": true,
  "data": null,
  "message": "User updated successfully",
  "errors": []
}
```

### Errors

| Status | message | Khi nào |
|--------|---------|---------|
| `404` | `"User not found"` | ID không tồn tại |
| `400` | `"Invalid phone number"` | PhoneNumber không đúng format |

---

## DELETE /api/users/{id}

Xóa user theo ID.

**Auth:** Yêu cầu Bearer Token

### Path Parameters

| Param | Type | Mô tả |
|-------|------|-------|
| `id` | `guid` | User ID |

### Response `200 OK`

```json
{
  "success": true,
  "data": null,
  "message": "User deleted successfully",
  "errors": []
}
```

### Errors

| Status | message | Khi nào |
|--------|---------|---------|
| `404` | `"User not found"` | ID không tồn tại |

---

# Roles API

Tất cả endpoint yêu cầu `Authorization: Bearer {token}`.

---

## GET /api/roles

Lấy danh sách role có phân trang và tìm kiếm.

**Auth:** Yêu cầu Bearer Token

### Query Parameters

| Param | Type | Default | Validation | Mô tả |
|-------|------|---------|------------|-------|
| `keyword` | `string?` | - | - | Tìm trong name và description |
| `name` | `string?` | - | - | Lọc chính xác theo name |
| `sortBy` | `string?` | - | `Name \| Description \| CreatedDate` | Trường sắp xếp |
| `sortDescending` | `boolean` | `false` | - | Sắp xếp giảm dần |
| `page` | `int` | `1` | ≥ 1 | Số trang (bắt đầu từ 1) |
| `pageSize` | `int` | `10` | 1–100 | Số item mỗi trang |

### Response `200 OK`

```json
{
  "success": true,
  "data": {
    "items": [ /* RoleDto[] */ ],
    "totalCount": 5,
    "pageIndex": 1,
    "pageSize": 10,
    "totalPages": 1,
    "hasPreviousPage": false,
    "hasNextPage": false
  },
  "message": "Success",
  "errors": []
}
```

---

## GET /api/roles/{id}

Lấy thông tin role theo ID.

**Auth:** Yêu cầu Bearer Token

### Path Parameters

| Param | Type | Mô tả |
|-------|------|-------|
| `id` | `guid` | Role ID |

### Response `200 OK`

```json
{
  "success": true,
  "data": { /* RoleDto */ },
  "message": "Success",
  "errors": []
}
```

### Errors

| Status | message | Khi nào |
|--------|---------|---------|
| `404` | `"Role not found"` | ID không tồn tại |

---

## POST /api/roles

Tạo role mới, tuỳ chọn gán permissions ngay khi tạo.

**Auth:** Yêu cầu Bearer Token

### Request Body

```json
{
  "roleName": "string",            // Required
  "description": "string",         // Optional
  "permissionIds": ["guid", "..."] // Optional — bỏ qua hoặc [] = không gán permission nào
}
```

### Validation Rules

| Field | Ràng buộc |
|-------|-----------|
| `roleName` | Required, max 50 ký tự |
| `permissionIds` | Optional; mỗi GUID phải là Permission ID đang tồn tại trong hệ thống |

### Response `201 Created`

Header: `Location: /api/roles/{newRoleId}`

```json
{
  "success": true,
  "data": { "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6" },
  "message": "Role created successfully",
  "errors": []
}
```

### Errors

| Status | message | Khi nào |
|--------|---------|---------|
| `400` | `"Validation failed"` | Field không hợp lệ |
| `409` | `"Role already exists"` | Tên role đã tồn tại |
| `422` | `"Permission(s) not found: <id1>, <id2>"` | Một hoặc nhiều permissionId không tồn tại |

---

## PUT /api/roles/{id}

Cập nhật thông tin role. Hỗ trợ sync permissions cho role.

**Auth:** Yêu cầu Bearer Token

### Path Parameters

| Param | Type | Mô tả |
|-------|------|-------|
| `id` | `guid` | Role ID |

### Request Body

```json
{
  "name": "string | null",            // Optional — bỏ qua = giữ nguyên
  "description": "string | null",     // Optional — bỏ qua = giữ nguyên
  "permissionIds": ["guid", "..."]    // Optional — xem bảng semantics bên dưới
}
```

### Semantics của `permissionIds`

| Giá trị | Hành vi |
|---------|---------|
| Không truyền / `null` | Permissions **không thay đổi** |
| `[]` (mảng rỗng) | **Xóa toàn bộ** permissions của role |
| `["id1", "id2"]` | **Sync**: thêm ID chưa có, xóa ID không còn trong danh sách |

### Response `200 OK`

```json
{
  "success": true,
  "data": null,
  "message": "Role updated successfully",
  "errors": []
}
```

### Errors

| Status | message | Khi nào |
|--------|---------|---------|
| `404` | `"Role not found"` | ID không tồn tại |
| `409` | `"Role name already exists"` | Tên mới đã được dùng |
| `422` | `"Permission(s) not found: <id1>, <id2>"` | Một hoặc nhiều permissionId không tồn tại |

---

## DELETE /api/roles/{id}

Xóa role theo ID.

**Auth:** Yêu cầu Bearer Token

### Path Parameters

| Param | Type | Mô tả |
|-------|------|-------|
| `id` | `guid` | Role ID |

### Response `200 OK`

```json
{
  "success": true,
  "data": null,
  "message": "Role deleted successfully",
  "errors": []
}
```

### Errors

| Status | message | Khi nào |
|--------|---------|---------|
| `404` | `"Role not found"` | ID không tồn tại |
| `400` | `"Role is in use"` | Role đang được gán cho user |
| `400` | `"Cannot delete admin role"` | Không được xóa role ADMIN |

---

# Permissions API

---

## POST /api/permissions/scan

Quét tất cả controller trong assembly, phát hiện các permission chưa có trong DB và đồng bộ vào DB.

**Auth:** Yêu cầu Bearer Token

Permission code được tạo theo convention: `{CONTROLLER_NAME}.{ACTION_NAME}` (uppercase).

Ví dụ: `AuthController.Login` → `AUTH.LOGIN`, `UsersController.Create` → `USERS.CREATE`

### Response `200 OK`

```json
{
  "success": true,
  "data": null,
  "message": "Permissions scanned and synchronised successfully",
  "errors": []
}
```

### Errors

| Status | Khi nào |
|--------|---------|
| `401` | Token không hợp lệ |
| `500` | Lỗi khi scan hoặc lưu DB |

---

# JWT Token Structure

## Access Token Claims

| Claim | Type | Giá trị |
|-------|------|---------|
| `nameid` (`ClaimTypes.NameIdentifier`) | `string` | User ID (GUID) |
| `unique_name` (`ClaimTypes.Name`) | `string` | UserName |
| `email` (`ClaimTypes.Email`) | `string` | Email |
| `permission` | `string` (multi-value) | Permission codes, ví dụ `USERS.CREATE` |
| `exp` | `number` | Unix timestamp hết hạn |
| `iss` | `string` | Issuer (`ProjectCore`) |
| `aud` | `string` | Audience (`ProjectCore`) |

**Algorithm:** HS256  
**TTL:** Cấu hình qua `JwtSettings:AccessTokenExpirationMinutes` (default: 60 phút)

## Refresh Token

- Format: 64-byte random base64 string
- TTL: 7 ngày
- Storage: In-memory `ConcurrentDictionary` (**TODO:** migrate sang persistent store)
- Behavior: Rotate mỗi lần dùng (old token bị hủy, new token được tạo)

---

# Error Response Examples

### 400 Validation Error

```json
{
  "success": false,
  "data": null,
  "message": "Validation failed",
  "errors": [
    "The UserName field is required.",
    "The Email field is not a valid e-mail address."
  ]
}
```

### 401 Unauthorized

```json
{
  "success": false,
  "data": null,
  "message": "Invalid credentials",
  "errors": []
}
```

### 404 Not Found

```json
{
  "success": false,
  "data": null,
  "message": "User not found",
  "errors": []
}
```

### 409 Conflict

```json
{
  "success": false,
  "data": null,
  "message": "Email already exists",
  "errors": []
}
```

### 422 Unprocessable Entity

```json
{
  "success": false,
  "data": null,
  "message": "Permission(s) not found: 3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "errors": []
}
```

### 500 Internal Server Error

```json
{
  "success": false,
  "data": null,
  "message": "An unexpected error occurred",
  "errors": []
}
```

---

# Luồng hoàn chỉnh: Setup ban đầu

```
1. POST /api/auth/login                            → Lấy accessToken
2. POST /api/permissions/scan                      → Đồng bộ permissions vào DB
3. GET  /api/roles                                 → Lấy ID của role ADMIN
4. GET  /api/permissions (hoặc xem DB)             → Lấy danh sách permissionId cần gán
5. PUT  /api/roles/{adminRoleId}                   → Gán permissions cho ADMIN role
       { "permissionIds": ["id1", "id2", "..."] }
6. Sẵn sàng sử dụng
```

# Luồng hoàn chỉnh: Quản lý User

```
1. GET  /api/users/roles         → Lấy danh sách role để hiển thị dropdown
2. POST /api/users               → Tạo user mới
3. GET  /api/users/{id}          → Lấy chi tiết user
4. PUT  /api/users/{id}          → Cập nhật profile
5. GET  /api/users?keyword=...   → Tìm kiếm / lọc user
6. DELETE /api/users/{id}        → Xóa user
```
