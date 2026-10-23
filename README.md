# FixtureForge

Web tabanlı ilişkisel test verisi üretimi ve SQL performans analiz platformu.

**Hedef veri tabanı:** Microsoft SQL Server
**Durum:** Geliştirme aşamasında (1. Hafta tamamlandı)

## Mimari

Katmanlı mimari (doküman 6.3):
FixtureForge.Api → HTTP endpoint'leri, Swagger
FixtureForge.Application → İş kuralları, interface'ler, validator'lar
FixtureForge.Domain → Entity'ler
FixtureForge.Infrastructure → Servis implementasyonları
FixtureForge.Persistence → EF Core, DbContext, migration'lar
FixtureForge.Contracts → DTO'lar
FixtureForge.Tests → xUnit testleri
fixtureforge-web → React + TypeScript (Vite)

## Gereksinimler

- .NET 10 SDK
- Node.js 20+ (npm 11+)
- Docker Desktop
- Git

## Kurulum

### 1. SQL Server (Docker)

docker compose up -d
docker compose ps   # healthy olana kadar bekle

### 2. Backend

cd FixtureForge.Api
dotnet run
API: http://localhost:5176
Swagger: http://localhost:5176/swagger

### 3. Frontend

cd fixtureforge-web
npm install
npm run dev
UI: http://localhost:5173

### Veritabanı
Connection string FixtureForge.Api/appsettings.Development.json içinde:

Server=localhost,1433;Database=FixtureForge;User Id=sa;Password=FixtureForge_Strong!Pass123;TrustServerCertificate=True;Encrypt=False

### Migration:
dotnet ef migrations add <Name> --project FixtureForge.Persistence --startup-project FixtureForge.Api
dotnet ef database update --project FixtureForge.Persistence --startup-project FixtureForge.Api

### Test
dotnet test

### Katkı
Commit mesajları: feat:, fix:, docs:, chore:, test:
Branch: main (korumalı), özellikler feature/<ad> branch'inde

---
