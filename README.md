# SecureDocumentVault

A security-focused .NET 8 Web API for encrypted document management. Documents are protected with AES-256 encryption, SHA-256 integrity hashing, and RSA digital signatures. Access is controlled via JWT authentication and role-based authorization.

---

## Architecture

```
Controller → Service (IService) → Repository (IRepository) → AppDbContext (EF Core)
```

```
FirstApi/
├── Controllers/        # HTTP layer — AuthController, DocumentController, AdminController, AuditLogController
├── Business/Services/  # Business logic + service interfaces
├── Repositories/       # Data access + repository interfaces
├── Data/               # AppDbContext, SeedData
├── Dtos/               # Request/response models with validation attributes
├── Models/             # EF Core entities (User, Document, Role, AuditLog)
└── Middlewares/        # ExceptionMiddleware — centralized error handling
```

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | .NET 8 Web API |
| ORM | Entity Framework Core |
| Database | SQL Server 2022 |
| Auth | JWT Bearer |
| Password hashing | BCrypt |
| Encryption | AES-256 (CBC) |
| Integrity | SHA-256 |
| Signing | RSA (PKCS#1) |
| Container | Docker / Docker Compose |

---

## Crypto Flow

### Document Creation
1. **Encrypt** — Content is encrypted with AES-256-CBC using a random IV. The IV is prepended to the ciphertext before storage.
2. **Hash** — A SHA-256 hash of the original plaintext content is computed and stored alongside the document.
3. **Sign** — The SHA-256 hash is signed with the RSA private key; the signature is stored in the `Signature` column.

### Verification (`POST /documents/{id}/verify`)
1. Decrypt stored ciphertext → recover plaintext.
2. Recompute SHA-256 of the plaintext.
3. Compare against stored hash — mismatch means the document was tampered with.

### Signature Verification (`POST /documents/{id}/verify-signature`)
1. Retrieve stored hash and signature.
2. Verify the signature against the hash using the RSA public key.

---

## Role Permissions

| Role | Permissions |
|---|---|
| **User** | Create and read own documents only |
| **Manager** | Read documents of users in their team (linked via `ManagerId`) |
| **Admin** | Full access to all documents + user/role management |
| **Auditor** | `verify` and `verify-signature` only — cannot read content or create documents |

---

## Endpoints

### Auth — `/auth`

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/auth/register` | Public | Register a new user |
| POST | `/auth/login` | Public | Login, returns JWT token |

### Documents — `/documents`

| Method | Route | Required Role | Description |
|---|---|---|---|
| POST | `/documents` | Admin, Manager, User | Create a new encrypted document |
| GET | `/documents` | Any authenticated | List accessible documents |
| GET | `/documents/{id}` | Admin, Manager, User | Get document with decrypted content |
| POST | `/documents/{id}/verify` | Any authenticated | Verify document integrity (hash check) |
| POST | `/documents/{id}/verify-signature` | Any authenticated | Verify RSA signature |

### Admin — `/admin`

| Method | Route | Required Role | Description |
|---|---|---|---|
| GET | `/admin/users` | Admin | List all users |
| PUT | `/admin/users/{id}/role` | Admin | Assign a role to a user |
| GET | `/admin/users/my-team` | Manager | List users in the manager's team |
| GET | `/admin/users/my-manager` | User | Get the user's assigned manager |

### Audit Logs — `/audit-logs`

| Method | Route | Required Role | Description |
|---|---|---|---|
| GET | `/audit-logs` | Admin, Auditor | List all audit log entries |
| GET | `/audit-logs/user/{userId}` | Admin, Auditor | List audit logs for a specific user |

---

## Setup

### Option A — Docker (recommended)

```bash
docker compose up --build
```

Swagger UI: [http://localhost:8080/swagger](http://localhost:8080/swagger)

### Option B — Manual

**Prerequisites:** .NET 8 SDK, SQL Server (or `docker run -e ACCEPT_EULA=Y -e SA_PASSWORD=... -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest`)

**1. Configure secrets**

```bash
dotnet user-secrets set "Jwt:Secret" "<your-256-bit-secret>"
dotnet user-secrets set "Crypto:Key" "<your-32-byte-base64-aes-key>"
dotnet user-secrets set "Signature:PrivateKey" "<RSA private key PEM>"
dotnet user-secrets set "Signature:PublicKey" "<RSA public key PEM>"
```

**2. Apply migrations and run**

```bash
dotnet ef database update
dotnet run
```

Swagger UI: [http://localhost:5000/swagger](http://localhost:5000/swagger)

---

## Seed Data

Seed runs automatically on startup.

| Role | Count | Email pattern | Password |
|---|---|---|---|
| Admin | 1 | Manually assigned via `/admin/users/{id}/role` | — |
| Manager | 5 | `manager1@test.com` … `manager5@test.com` | `Manager123!` |
| User | 50 | `user1_1@test.com` … `user5_10@test.com` | `User123!` |

Each manager has 10 users assigned via `ManagerId`.

---

## Example Requests

### POST `/auth/login`

```json
// Request
{
  "email": "manager1@test.com",
  "password": "Manager123!"
}

// Response 200
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "manager1@test.com"
}
```

### POST `/documents`

```json
// Request (Authorization: Bearer <token>)
{
  "title": "Q2 Financial Report",
  "content": "Revenue increased by 12% in Q2..."
}

// Response 200
"3fa85f64-5717-4562-b3fc-2c963f66afa6"
```
