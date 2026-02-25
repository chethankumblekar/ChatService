<div align="center">

# 💬 ChatService

**Real-time messaging backend — ASP.NET Core 9 · SignalR · Clean Architecture**

<br/>

![.NET](https://img.shields.io/badge/.NET_9-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23_13-239120?style=for-the-badge&logo=csharp&logoColor=white)
![SignalR](https://img.shields.io/badge/SignalR-0078D4?style=for-the-badge&logo=microsoft&logoColor=white)
![EF Core](https://img.shields.io/badge/EF_Core_9-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server_2022-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)
![Google OAuth](https://img.shields.io/badge/Google_OAuth-4285F4?style=for-the-badge&logo=google&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)

<br/>

[![Build](https://img.shields.io/github/actions/workflow/status/chethankumblekar/ChatService/dotnet.yml?branch=master&label=build&style=flat-square&logo=github)](https://github.com/chethankumblekar/ChatService/actions)
[![License](https://img.shields.io/badge/license-MIT-blue?style=flat-square)](LICENSE)

</div>

---

## 📐 Architecture

> See ![ChatService Architecture](docs/chatservice-architecture.svg)

```
┌─ ChatService.Api ──────────────────────────────────────────────┐
│  AuthController · UserController · ChatHub · ExceptionMiddleware│
└──────────────────────────────┬─────────────────────────────────┘
                               │ depends on interfaces only
┌─ ChatService.Application ────▼─────────────────────────────────┐
│  Commands · Queries · DTOs · IAuthService · IChatService        │
└──────────────────────────────┬─────────────────────────────────┘
                               │ domain models only
┌─ ChatService.Domain ─────────▼─────────────────────────────────┐
│  User · Message · Group · IUserRepository · IMessageRepository  │
└──────────────────────────────┬─────────────────────────────────┘
                               │ implements interfaces
┌─ ChatService.Infrastructure ─▼─────────────────────────────────┐
│  EF Core · SQL Server · Repositories · AuthService              │
└────────────────────────────────────────────────────────────────┘
```

**Dependency rule:** each layer depends only on the layer below it. Infrastructure implements Domain interfaces — Domain never references Infrastructure.

---

## 🗂 Project Structure

```
ChatService/
├── ChatService.sln
├── docker-compose.yml
├── .gitignore
│
├── src/
│   ├── ChatService.Api/                    # 🌐 Entry point
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs           # POST /api/auth/google
│   │   │   └── UserController.cs           # GET /api/user, /conversations, /messages
│   │   ├── Hubs/
│   │   │   └── ChatHub.cs                  # SignalR — messaging, presence, typing
│   │   ├── Middleware/
│   │   │   └── ExceptionMiddleware.cs      # Global exception → RFC 7807 response
│   │   ├── Helpers/
│   │   │   └── JwtUserIdProvider.cs        # sub claim → SignalR userId
│   │   ├── Extensions/
│   │   ├── Properties/launchSettings.json
│   │   ├── Program.cs                      # DI, JWT, CORS, Swagger, Health
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   └── Dockerfile
│   │
│   ├── ChatService.Application/            # ⚙️ Business logic
│   │   ├── Commands/
│   │   │   └── SendMessage/
│   │   │       └── SendDirectMessageCommand.cs
│   │   ├── Queries/
│   │   │   ├── GetMessages/
│   │   │   │   ├── GetDirectMessagesQuery.cs
│   │   │   │   └── GetDirectMessagesHandler.cs
│   │   │   └── GetConversations/
│   │   │       ├── GetConversationsQuery.cs
│   │   │       └── GetConversationsHandler.cs
│   │   ├── DTOs/
│   │   │   ├── AuthDtos.cs                 # GoogleAuthRequest, AuthResponse
│   │   │   ├── MessageDto.cs               # MessageDto, ConversationDto
│   │   │   └── UserDto.cs
│   │   ├── Interfaces/
│   │   │   ├── IAuthService.cs
│   │   │   └── IChatService.cs
│   │   └── Mappings/
│   │       └── MessageMappings.cs          # Message → MessageDto
│   │
│   ├── ChatService.Domain/                 # 🏛 Core domain (no external deps)
│   │   ├── Entities/
│   │   │   ├── BaseEntity.cs
│   │   │   ├── User.cs                     # Email as PK, SentMessages, ReceivedMessages
│   │   │   ├── Message.cs                  # CreateDirect, CreateGroup, SoftDelete
│   │   │   └── Group.cs                    # AddMember, RemoveMember
│   │   ├── Enums/
│   │   │   └── MessageType.cs              # Direct = 1, Group = 2
│   │   ├── Exceptions/
│   │   │   └── DomainException.cs          # NotFoundException, UnauthorizedException
│   │   └── Interfaces/
│   │       ├── IUserRepository.cs
│   │       ├── IMessageRepository.cs       # ConversationSummary record
│   │       └── IGroupRepository.cs
│   │
│   └── ChatService.Infrastructure/         # 🔧 Data access & external services
│       ├── Persistence/
│       │   ├── AppDbContext.cs             # EF Core, auto UpdatedAt
│       │   ├── Configurations/
│       │   │   ├── UserConfiguration.cs    # Explicit FK mapping (fixes nav ambiguity)
│       │   │   ├── MessageConfiguration.cs # Soft delete global filter
│       │   │   └── GroupConfiguration.cs   # Many-to-many group_members
│       │   └── Repositories/
│       │       ├── UserRepository.cs
│       │       ├── MessageRepository.cs
│       │       └── GroupRepository.cs
│       ├── Services/
│       │   ├── AuthService.cs              # Google validation + JWT issue
│       │   └── ChatServiceImpl.cs          # IChatService implementation
│       └── Extensions/
│           └── ServiceCollectionExtensions.cs  # AddInfrastructure()
│
└── tests/
    └── ChatService.Tests/
        └── Domain/
            └── MessageTests.cs             # xUnit + FluentAssertions + Moq
```

---

## 🍎 Mac M2 Development Setup

### 1 — Homebrew

```bash
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"

# Add to PATH (Apple Silicon)
echo 'eval "$(/opt/homebrew/bin/brew shellenv)"' >> ~/.zprofile
eval "$(/opt/homebrew/bin/brew shellenv)"
```

### 2 — .NET 9 SDK

```bash
# Option A — Official installer (recommended for M2)
# Download from https://dotnet.microsoft.com/download/dotnet/9.0
# Choose: macOS · Arm64 · SDK installer

# Option B — Homebrew
brew install --cask dotnet-sdk

# Verify
dotnet --version   # should print 9.x.x
dotnet --info      # should show arm64 architecture
```

### 3 — Docker Desktop for Mac (Apple Silicon)

```bash
# Download from https://www.docker.com/products/docker-desktop/
# Choose: Mac with Apple chip (.dmg)

# Or via Homebrew
brew install --cask docker

# Verify
docker --version
docker compose version
```

### 4 — SQL Server 2022 on Docker (M2 compatible)

> ⚠️ Microsoft's official SQL Server image does **not** support arm64. Use the Azure SQL Edge image instead — it is fully compatible with Apple Silicon and has an identical API.

```bash
# Pull the arm64-compatible image
docker pull mcr.microsoft.com/azure-sql-edge:latest

# Run SQL Server (Azure SQL Edge) on port 1433
docker run -d \
  --name chatservice-db \
  -e "ACCEPT_EULA=1" \
  -e "MSSQL_SA_PASSWORD=YourStrong!Passw0rd" \
  -e "MSSQL_PID=Developer" \
  -p 1433:1433 \
  --restart unless-stopped \
  mcr.microsoft.com/azure-sql-edge:latest

# Verify it's running
docker ps
docker logs chatservice-db
```

Or use Docker Compose (already included in the repo):

```bash
# Update docker-compose.yml image line to:
# image: mcr.microsoft.com/azure-sql-edge:latest
docker compose up -d db
```

**Connection string for `appsettings.json`:**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=ChatService;User ID=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;"
}
```

### 5 — IDE: Rider or VS Code

**JetBrains Rider** (recommended):
```bash
brew install --cask rider
```

**VS Code** with extensions:
```bash
brew install --cask visual-studio-code

# Install extensions
code --install-extension ms-dotnettools.csdevkit
code --install-extension ms-dotnettools.csharp
code --install-extension formulahendry.dotnet-test-explorer
code --install-extension ms-azuretools.vscode-docker
```

### 6 — EF Core Tools

```bash
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef

# Verify
dotnet ef --version
```

### 7 — Azure Data Studio (SQL client for Mac)

```bash
brew install --cask azure-data-studio
# Connect to: localhost,1433 · SA · YourStrong!Passw0rd
```

---

## 🚀 Running the Project

### Clone and configure secrets

```bash
git clone https://github.com/chethankumblekar/ChatService.git
cd ChatService

# Set secrets (never commit these values)
cd src/ChatService.Api
dotnet user-secrets init
dotnet user-secrets set "Authentication:JwtSecret" "$(openssl rand -hex 32)"
dotnet user-secrets set "Authentication:GoogleClientId" "YOUR_ID.apps.googleusercontent.com"
```

### Start the database

```bash
# From repo root
docker compose up -d db

# Wait a few seconds for SQL Server to start, then run migrations
cd src/ChatService.Infrastructure
dotnet ef database update --startup-project ../ChatService.Api
```

### Run the API

```bash
cd src/ChatService.Api
dotnet run

# API: https://localhost:7058
# Swagger: https://localhost:7058/swagger
# Health: https://localhost:7058/health
```

### Run tests

```bash
cd tests/ChatService.Tests
dotnet test --verbosity normal
```

---

## 📡 API Reference

### 🔐 Auth endpoints

| Method | Endpoint | Auth | Body | Response |
|--------|----------|:----:|------|----------|
| `POST` | `/api/auth/google` | ❌ | `{ "token": "<google_id_token>" }` | `{ "token": "<app_jwt>" }` |
| `GET`  | `/api/auth/me`     | ✅ | — | JWT claims as JSON object |

### 👤 User endpoints

| Method | Endpoint | Auth | Query | Response |
|--------|----------|:----:|-------|----------|
| `GET` | `/api/user` | ✅ | `?search=name` | `UserDto[]` |
| `GET` | `/api/user/conversations` | ✅ | — | `ConversationDto[]` |
| `GET` | `/api/user/messages/{recipientId}` | ✅ | `?skip=0&take=50` | `MessageDto[]` |
| `POST` | `/api/user/messages/{senderId}/read` | ✅ | — | `204 No Content` |

### ❤️ Health

| Endpoint | Description |
|----------|-------------|
| `GET /health` | Returns `Healthy` if DB connection is alive |
| `GET /swagger` | Swagger UI (development only) |

---

## ⚡ SignalR Hub — `/hubs/chat`

**Connect:** Pass JWT in query string — `wss://localhost:7058/hubs/chat?access_token=<jwt>`

### Client → Server (invoke)

| Method | Parameters | Description |
|--------|-----------|-------------|
| `SendMessageToUser` | `recipientId: string, content: string` | Send direct message |
| `SendMessageToGroup` | `groupId: string, content: string` | Send to group |
| `JoinGroup` | `groupId: string` | Subscribe to group events |
| `LeaveGroup` | `groupId: string` | Unsubscribe from group |
| `MarkMessageRead` | `messageId: Guid` | Confirm message read |
| `Typing` | `recipientId: string` | Send typing indicator |
| `IsUserOnline` | `userId: string` | Returns `bool` |

### Server → Client (on)

| Event | Payload | Description |
|-------|---------|-------------|
| `ReceiveMessage` | `{ id, senderId, recipientId, content, sentAt }` | New DM arrived |
| `MessageSent` | `{ id, senderId, recipientId, content, sentAt }` | Echo to sender's other tabs |
| `ReceiveGroupMessage` | `{ id, senderId, groupId, content, sentAt }` | New group message |
| `MessageRead` | `messageId: string, readerId: string` | Read receipt from recipient |
| `UserOnline` | `userId: string` | User came online |
| `UserOffline` | `userId: string` | User went offline |
| `OnlineUsers` | `string[]` | Full presence list sent on connect |
| `UserTyping` | `userId: string` | Typing indicator from user |

---

## 🗄️ Database Schema

```
users
  email          PK  varchar(256)
  first_name         varchar(50)   NOT NULL
  last_name          varchar(50)   NOT NULL
  avatar_url         varchar(2048) NULL
  last_seen_at       datetime2
  created_at         datetime2
  updated_at         datetime2

messages
  id             PK  uniqueidentifier
  sender_id      FK→ users.email     NOT NULL
  recipient_id   FK→ users.email     NULL  (null = group message)
  group_id       FK→ groups.id       NULL  (null = direct message)
  content            varchar(4000)   NOT NULL
  sent_at            datetime2       NOT NULL
  read_at            datetime2       NULL
  is_deleted         bit             DEFAULT 0  (soft delete)
  type               int             1=Direct 2=Group
  created_at         datetime2
  updated_at         datetime2

groups
  id             PK  uniqueidentifier
  name               varchar(100)   NOT NULL
  description        varchar(500)
  created_by_id  FK→ users.email
  created_at         datetime2
  updated_at         datetime2

group_members (join table)
  groups_id      FK→ groups.id
  members_email  FK→ users.email
```

---

## ⚙️ Configuration Reference

```json
{
  "Authentication": {
    "JwtSecret":          "min-32-char-random-secret — use: openssl rand -hex 32",
    "GoogleClientId":     "123456789.apps.googleusercontent.com",
    "Issuer":             "chatchatni",
    "Audience":           "chatchatni",
    "TokenExpiryMinutes": 60
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "https://your-app.vercel.app"
    ]
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=ChatService;User ID=sa;Password=...;TrustServerCertificate=True;"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": { "Microsoft": "Warning", "System": "Warning" }
    }
  }
}
```

> 🔑 **Never commit secrets.** Use `dotnet user-secrets` in development and environment variables / Azure Key Vault in production.

---

## 🐛 15 Bugs Fixed

| # | Bug | Impact | Fix |
|---|-----|--------|-----|
| 1 | Repos registered as `Singleton`, DbContext is `Scoped` | Crash under concurrent load | All repos changed to `AddScoped` |
| 2 | `DeleteGroup` threw on success | Groups couldn't be deleted | Moved throw inside null check |
| 3 | Hub `SendMessageToUser` returned `void` | No message ID returned | Returns `MessageDto` with persisted ID |
| 4 | Presence tracked by `ConnectionId` only | Multi-tab broken | `ConcurrentDictionary<userId, HashSet<connId>>` |
| 5 | Sender's other tabs got no message echo | Multi-device broken | Added `MessageSent` back to sender |
| 6 | SMTP email validation | Unreliable + security issue | Replaced with RFC 5322 regex |
| 7 | Missing `return BadRequest()` | Auth failures returned HTTP 200 | Added `return` keyword |
| 8 | Wrong JWT claim in `JwtUserIdProvider` | `Clients.User()` never matched | Changed to `sub` claim |
| 9 | Wrong SignalR NuGet package (OWIN v2) | App crashed on startup | Replaced with ASP.NET Core SignalR |
| 10 | CORS only allowed `localhost:3000` | Production blocked | Config-driven `Cors:AllowedOrigins` array |
| 11 | EF navigation property ambiguity | Startup crash | Explicit lambda FK mapping in configurations |
| 12 | `GroupBy` attempted in EF translation | Runtime `InvalidOperationException` | Client-side grouping after `.ToListAsync()` |
| 13 | Google token audience not validated | Any Google app's token accepted | Added `Audience` to `ValidationSettings` |
| 14 | `DefaultInboundClaimTypeMap` remapped `sub` | `FindFirstValue(Sub)` returned null | Added `JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear()` |
| 15 | Middleware order: Auth before CORS | CORS preflight returned 401 | Correct order: CORS → Auth → Authz |

---

## 🧪 Tests

```bash
cd tests/ChatService.Tests
dotnet test --verbosity normal

# With coverage (requires coverlet)
dotnet test --collect:"XPlat Code Coverage"
```

**Stack:** xUnit · FluentAssertions · Moq

---

## 🐳 Docker

```bash
# Run everything (API + SQL Server)
docker compose up --build

# API: http://localhost:8080
# Swagger: http://localhost:8080/swagger

# Run only the database
docker compose up -d db
```

**Environment variables for production:**

```bash
Authentication__JwtSecret=your-secret
Authentication__GoogleClientId=your-client-id
ConnectionStrings__DefaultConnection=your-connection-string
Cors__AllowedOrigins__0=https://your-app.vercel.app
```

---

## 📄 License

MIT © [Chethan Kumblekar](https://github.com/chethankumblekar)
