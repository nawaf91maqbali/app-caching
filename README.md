# 🔥 ASP.NET Core Web API – Caching with In-Memory, Redis, and SQLite

This project demonstrates **multi-layered caching** in an ASP.NET Core Web API using:

- ✅ **In-Memory Cache**
- ✅ **Redis Distributed Cache**
- ✅ **SQLite** as the primary database

---

## 📦 Technologies Used

- ASP.NET Core Web API (.NET 9)
- `Microsoft.Extensions.Caching.Memory`
- `Microsoft.Extensions.Caching.StackExchangeRedis`
- `Microsoft.EntityFrameworkCore.Sqlite`
- Docker (for Redis)

---

## 📁 Project Structure

```bash
├── Controllers/
│   └── UserController.cs
├── Data/
│   ├── AppDbContext.cs
│   └── AppDbSeed.cs
├── Models/
│   └── User.cs
├── Redis/
│   └── docker-compose.redis.yml
├── Services/
│   ├── CacheService.cs
│   └── UserService.cs
├── Shared/
│   └── Enums.cs      # Enum for selecting cache provider
├── appsettings.json
├── Program.cs
└── README.md
```

---

## ⚙️ Configuration (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "SqlLite": "Data Source=default.db"
  },
  "CacheSettings": {
    "CacheType": "Redis",
    "ExpirationTime": 5,
    "RedisConnectionString": "localhost:6379"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

---

## 🐳 Redis Setup with Docker Compose

> Located at `Redis/docker-compose.redis.yml`

```yaml
version: '3.8'

name: "redis-server"
services:
  redis:
    image: redis:latest
    container_name: redis
    ports:
      - "6379:6379"
    restart: always
    volumes:
      - redis-data:/data
    command: ["redis-server", "--appendonly", "yes"]

volumes:
  redis-data:
```

### 🔹 Start Redis

```bash
cd Redis
docker-compose -f docker-compose.redis.yml up -d
```

---

## 🔧 Setup & Run Instructions

### ✅ 1. Install Dependencies

```bash
dotnet restore
```

---

### ✅ 2. Apply EF Core Migrations

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

> Ensure the `AppDbContext` is properly configured with `Sqlite` in `Program.cs`.

---

### ✅ 3. Start Redis (if not already running)

```bash
docker-compose -f Redis/docker-compose.redis.yml up -d
```

---

### ✅ 4. Run the Web API

```bash
dotnet run
```

Visit:
```
https://localhost:<port>/swagger
```

---

## 🔐 Caching Behavior

The cache provider is selected based on the value of `CacheType` from configuration:

| `CacheType`     | Description                          |
|-----------------|--------------------------------------|
| `"InMemory"`     | Uses local in-process memory cache   |
| `"Redis"`        | Uses distributed Redis cache         |

### 🧠 Expiration Time

- Controlled by `ExpirationTimeInMinutes` in `appsettings.json`
- Applies to both Redis and In-Memory

---

## 🧪 Example API

### ➤ `GET /api/user`

- Fetches all users
- Caches the result based on the configured cache provider

---

## 📦 Seed Initial Data

In `AppDbSeed.cs`, data is seeded on app startup if not present.

---

## 💡 Tips

- Use `CacheService` to abstract between Redis and Memory
- Use `UserService` to encapsulate DB + cache logic
- Use `Enum CacheType { InMemory, Redis }` to toggle logic easily

---

## 🙋‍♂️ Author

**Nawaf AL-Maqbali**  
📧 [LinkedIn](https://www.linkedin.com/in/nawaf-al-maqbali-6bb4a6227)
