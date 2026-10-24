# FixtureForge

**A relational test data generation and SQL performance analysis platform for Microsoft SQL Server.**

FixtureForge is an open-source developer tool designed to help developers define relational database schemas, model table relationships, and generate structured test data while preserving referential integrity.

> **Status:** Under active development  
> **Target Database:** Microsoft SQL Server  
> **Current Focus:** Schema management and relational test data generation

## Overview

Creating realistic test data for relational databases can be tedious, especially when multiple tables depend on each other through foreign key relationships.

FixtureForge aims to simplify this process by providing a web-based environment where developers can define database schemas, configure relationships, generate synthetic test data, and export SQL scripts for use in their development and testing workflows.

The project is being developed with a layered .NET backend and a React + TypeScript frontend.

## Features

### Schema Management
- Create and manage projects.
- Define database tables and columns.
- Configure data types and column constraints.
- Define relationships between tables using foreign keys.
- Track schema changes through schema versioning.

### Relational Test Data Generation
- Configure data generation jobs for project tables.
- Generate synthetic data for relational schemas.
- Support reproducible generation through random seeds.
- Track generation status and per-table results.
- Preserve referential integrity across related tables.

### SQL Export
- Export generated data as SQL `INSERT` statements.
- Include generation metadata such as job ID, seed, schema version, and generator version.

### Development Infrastructure
- Layered backend architecture.
- Entity Framework Core persistence and migrations.
- Request validation.
- Automated builds and tests through GitHub Actions.
- xUnit test suite.

*Features are being implemented and validated incrementally. Availability may vary as development progresses.*

## Architecture

FixtureForge follows a layered architecture to separate domain logic, application rules, infrastructure concerns, persistence, and API contracts.

```text
FixtureForge/
├── FixtureForge.Api/             # HTTP endpoints and Swagger
├── FixtureForge.Application/    # Application rules, interfaces, validators
├── FixtureForge.Domain/         # Domain entities
├── FixtureForge.Infrastructure/# Service implementations
├── FixtureForge.Persistence/    # EF Core, DbContext, migrations
├── FixtureForge.Contracts/      # DTOs and API contracts
├── FixtureForge.Tests/          # Automated tests (xUnit)
└── fixtureforge-web/             # React + TypeScript frontend
```

### Technology Stack

| Layer | Technologies |
|---|---|
| Backend | C#, ASP.NET Core, .NET 10 |
| ORM | Entity Framework Core |
| Database | Microsoft SQL Server |
| Frontend | React, TypeScript, Vite |
| Testing | xUnit |
| API Documentation | Swagger / OpenAPI |
| CI | GitHub Actions |
| Local Database | Docker Compose |

## Getting Started

### Prerequisites

Install the following tools before running FixtureForge locally:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js](https://nodejs.org/) 20 or later
- npm 11 or later
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Git](https://git-scm.com/)

### 1. Clone the Repository

```bash
git clone https://github.com/nazicenn/Fixture-Forge.git
cd FixtureForge
```

Make sure you enter the repository directory created by Git. If the directory name differs, use that directory instead.

### 2. Start SQL Server

Start the SQL Server container from the repository root:

```bash
docker compose up -d
docker compose ps
```

Wait until the database container reports a healthy status before starting the backend.

### 3. Configure the Connection String

Configure the development connection string in:

`FixtureForge.Api/appsettings.Development.json`

Example for a local Docker SQL Server instance:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=FixtureForge;User Id=sa;Password=YOUR_LOCAL_PASSWORD;TrustServerCertificate=True;Encrypt=False"
  }
}
```

Use the connection-string key expected by your application if it differs from `DefaultConnection`.

**Security note:** Never commit real database credentials, API keys, or other secrets to source control. Keep local credentials out of tracked files and use environment variables or an appropriate secrets-management mechanism where possible.

### 4. Apply Database Migrations

Run the following commands from the repository root:

```bash
dotnet ef migrations add <MigrationName> --project FixtureForge.Persistence --startup-project FixtureForge.Api
dotnet ef database update --project FixtureForge.Persistence --startup-project FixtureForge.Api
```

Replace `<MigrationName>` with a meaningful migration name. Create a migration only when your model changes require one.

If the `dotnet ef` command is unavailable, install the Entity Framework Core command-line tool and ensure its version is compatible with your project.

### 5. Run the Backend

```bash
cd FixtureForge.Api
dotnet run
```

Local development endpoints:

- **API:** http://localhost:5176
- **Swagger UI:** http://localhost:5176/swagger

These URLs assume the application is configured to use port `5176`.

### 6. Run the Frontend

Open a separate terminal from the repository root:

```bash
cd fixtureforge-web
npm install
npm run dev
```

Open the frontend at:

http://localhost:5173

### 7. Run Tests

From the repository root:

```bash
dotnet test
```

## Development Workflow

FixtureForge uses feature branches and conventional commit prefixes to keep changes organized.

### Branch Naming

Create a separate branch for each feature or fix:

```bash
git switch -c feature/<feature-name>
```

Examples:

```text
feature/generation-jobs
feature/sql-export
fix/foreign-key-validation
docs/readme-improvements
```

### Commit Messages

Use the following prefixes:

| Prefix | Purpose |
|---|---|
| `feat:` | New feature |
| `fix:` | Bug fix |
| `docs:` | Documentation changes |
| `chore:` | Maintenance and tooling |
| `test:` | Tests |

Examples:

```bash
git commit -m "feat: add relational data generation"
git commit -m "fix: validate foreign key targets"
git commit -m "test: cover generation job validation"
```

Submit changes through a pull request when contributing to the project.

## Design Principles

- **Referential integrity:** Generated data should respect configured relationships.
- **Reproducibility:** A fixed seed should allow deterministic generation under the same applicable conditions.
- **Separation of concerns:** Each backend layer has a clearly defined responsibility.
- **Validation:** Invalid schemas and generation requests should be rejected appropriately.
- **Maintainability:** Features should be testable and independently extensible.
- **Security:** Secrets and project data must be handled with appropriate access controls as the platform evolves.

## Roadmap

Planned development areas include:

- [x] Project management foundation
- [x] Table and column schema management
- [x] Foreign key relationship management
- [x] Initial relational test data generation workflow
- [ ] Expand generation validation and edge-case coverage
- [ ] Improve SQL export validation and testing
- [ ] Additional export formats, including CSV and JSON
- [ ] SQL performance analysis capabilities
- [ ] Authentication and user-owned projects
- [ ] Further security hardening

*Roadmap items describe development direction and may change as the project evolves.*

## Contributing

Contributions, bug reports, and suggestions are welcome.

1. Create a feature branch.
2. Keep changes focused and follow the existing architecture.
3. Add or update tests when appropriate.
4. Run the test suite before submitting changes.
5. Use the project's commit-message conventions.
6. Open a pull request describing the change and its motivation.

## License

Add the project's license information here once the repository's license has been finalized.

---

**FixtureForge** — building a more reliable workflow for relational test data generation.
