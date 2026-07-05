# Falck — Employee Management API

Technical test for .NET Backend Developer. ASP.NET Core 9 Web API for managing
employees, departments, projects and position history, secured with JWT and
role-based authorization.

## Tech stack

- .NET 9 / ASP.NET Core Web API (controllers)
- Entity Framework Core 9 + SQL Server LocalDB
- JWT Bearer authentication, BCrypt password hashing
- Swagger (Swashbuckle) with Bearer support
- xUnit (25 unit tests)

## Getting started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server LocalDB (ships with Visual Studio; verify with `sqllocaldb info`).
  To use another SQL Server instance, change `ConnectionStrings:DefaultConnection`
  in `src/Falck.Api/appsettings.json`.

### Run

```bash
git clone <this-repo>
cd Falck
dotnet run --project src/Falck.Api --launch-profile http
```

That is all: on startup the API applies the EF Core migrations, creates the
`FalckEmployeesDb` database and seeds demo data (departments, projects,
employees with position history, and two demo accounts).

Open **http://localhost:5229/swagger** to explore the API.

### Demo accounts

| Username | Password    | Role  | Access                              |
|----------|-------------|-------|-------------------------------------|
| `admin`  | `Admin123!` | Admin | All employees endpoints             |
| `user`   | `User123!`  | User  | Only GET on the employees endpoints |

Login via `POST /api/auth/login`, copy the `token` from the response and use
the **Authorize** button in Swagger (or an `Authorization: Bearer <token>`
header). `src/Falck.Api/Falck.Api.http` contains ready-made sample requests.

### Run with Docker (no local SQL Server needed)

The `dotnet run` path above targets SQL Server **LocalDB** (Windows only). To run
anywhere, use Docker Compose, which starts a Linux SQL Server container plus the
API and wires them together:

```bash
docker compose up --build
```

Then open **http://localhost:8080/swagger**. Compose overrides the connection
string to point at the `db` service and waits (healthcheck) until SQL Server is
ready; the API then applies the migrations and seeds the same demo data. Stop
with `docker compose down` (add `-v` to also drop the database volume).

The `Dockerfile` is a multi-stage build (SDK to publish, ASP.NET runtime to run)
and `Jwt__Key` / connection string are supplied as environment variables — the
same override mechanism a real deployment would use for secrets.

### Tests

```bash
dotnet test
```

Coverage (hand-written code, excluding generated migrations):

```bash
dotnet test --settings coverlet.runsettings --collect:"XPlat Code Coverage"
```

## Endpoints

| Method | Route                                          | Roles       | Description |
|--------|------------------------------------------------|-------------|-------------|
| POST   | `/api/auth/register`                           | anonymous   | Create a read-only User account, returns a JWT |
| POST   | `/api/auth/login`                              | anonymous   | Authenticate, returns a JWT with the role claim |
| GET    | `/api/employees`                               | Admin, User | List employees (with computed yearly bonus) |
| GET    | `/api/employees/{id}`                          | Admin, User | Employee detail: position history + projects |
| POST   | `/api/employees`                               | Admin       | Create employee (opens its first history record) |
| PUT    | `/api/employees/{id}`                          | Admin       | Update; a position change is recorded in the history |
| DELETE | `/api/employees/{id}`                          | Admin       | Delete employee |
| GET    | `/api/departments`                             | Admin, User | List departments |
| GET    | `/api/departments/{id}/employees-with-projects`| Admin, User | Section 4.3 query over HTTP |

## Architecture

**Clean Architecture** with the dependency rule pointing inwards:

```
Falck.Api ──────────► Falck.Application ──────► Falck.Domain
    │                        ▲
    └──► Falck.Infrastructure┘   (implements Application/Domain contracts)
```

| Project | Responsibility | Depends on |
|---|---|---|
| `Falck.Domain` | Entities, business rules (bonus strategies, position-change rule) | nothing |
| `Falck.Application` | Use cases (services), DTOs, repository/port contracts | Domain |
| `Falck.Infrastructure` | EF Core, repositories, BCrypt, JWT generation | Application |
| `Falck.Api` | Controllers, middleware, DI composition, Swagger | Application, Infrastructure |

The API never touches EF Core directly; the application layer depends only on
interfaces (`IEmployeeRepository`, `IPasswordHasher`, `IJwtTokenGenerator`)
whose implementations live in Infrastructure.

### Design patterns

1. **Strategy** — `IBonusStrategy` (`Falck.Domain/Strategies`) encapsulates the
   bonus policy: `RegularEmployeeBonusStrategy` (10%) and
   `ManagerBonusStrategy` (20%, shared by every manager type). New policies are
   added without touching `Employee` (Open/Closed).
2. **Factory** — `BonusStrategyFactory` resolves the strategy from the
   employee's position, so callers never branch on position themselves.
3. **Repository** — persistence is abstracted behind `IEmployeeRepository` /
   `IDepartmentRepository` / `IUserRepository`; EF Core implementations live in
   `Falck.Infrastructure/Persistence/Repositories`. The `DbContext` doubles as
   the Unit of Work.

### SOLID

- **S** — thin controllers, one use-case service per aggregate, one strategy
  per bonus policy, one `IEntityTypeConfiguration` per entity.
- **O** — new manager types = one enum entry; new bonus policies = one new
  strategy class. Nothing existing changes.
- **L** — every `IBonusStrategy` is interchangeable wherever the interface is
  expected; the factory relies on that.
- **I** — small, focused contracts (`IPasswordHasher` has 2 methods) instead of
  one fat "IRepository of everything".
- **D** — Application depends on abstractions; EF Core, BCrypt and JWT are
  implementation details injected from the composition roots
  (`AddApplication()` / `AddInfrastructure()`).

### Design decisions worth calling out

- **`CurrentPosition` is an int-backed enum** (`PositionType`). The spec asks
  for `CurrentPosition (int)`; the enum persists as `int` while keeping the
  values type-safe. Manager types occupy a "managerial band" (values >= 10), so
  `IsManagerial()` covers *many types of managers* with one rule.
- **`PositionHistory.Position` is a string** exactly as the spec requires; the
  value is the `PositionType` name to stay consistent with the enum.
- **Position changes go through the domain** — `Employee.ChangePosition()` is
  the only way to change position: it closes the open history record and opens
  a new one. The service never manipulates the history by hand.
- **Custom `Users` table + BCrypt instead of ASP.NET Identity** — the test
  needs two roles and JWT issuance; full Identity would add 7 tables and hide
  the interesting parts (hashing, claims) behind magic.
- **Demo users seeded at startup (`DbSeeder`), not via `HasData`** — BCrypt
  emits a different hash per run, which would dirty every future migration.
- **Manual DTO mapping instead of AutoMapper** — the surface is small; explicit
  mapping stays compile-time safe.
- **Repo-local `nuget.config`** pinning nuget.org so the solution restores
  identically on any machine.

## Database schema (section 4.1)

Tables: `Employees`, `Departments` (1:N), `Projects` (N:N via
`EmployeeProjects`), `PositionHistories` (1:N, `EndDate NULL` = current
position), `Users`. Full DDL: [`docs/database-schema.sql`](docs/database-schema.sql).
Salary is `decimal(18,2)`; `PositionHistories` has a covering index on
`(EmployeeId, StartDate)`; `Users.Username` is unique.

The section 4.3 LINQ query lives in
`EmployeeRepository.GetByDepartmentWithProjectsAsync`:

```csharp
context.Employees
    .AsNoTracking()
    .Where(e => e.DepartmentId == departmentId && e.Projects.Any())
    .Include(e => e.Department)
    .Include(e => e.Projects)
    .ToListAsync(cancellationToken);
```

---

## Written answers

### 2.2 — How authentication and authorization are implemented

**Authentication** (who you are): the API issues stateless JWTs. `POST
/api/auth/login` verifies the BCrypt hash and returns a token signed with
HMAC-SHA256 containing `sub`, `unique_name`, `jti` and a role claim, with a
60-minute lifetime. The JWT bearer middleware validates signature, issuer,
audience and lifetime on every request and builds the `ClaimsPrincipal`. The
signing key lives in configuration for this demo; in production it would come
from a secret store (user-secrets, environment variables, Azure Key Vault) and
tokens would be paired with refresh tokens over HTTPS only.

**Authorization** (what you may do): role-based, driven by the role claim.
`EmployeesController` is decorated with `[Authorize(Roles = "Admin,User")]` and
the mutating actions tighten it to `[Authorize(Roles = "Admin")]`, which yields
exactly the section 3.2 matrix: Admin = full access, User = GET only. For finer
rules the same mechanism scales to policy-based authorization
(`AddAuthorization(options => options.AddPolicy(...))`) with custom
requirements/handlers.

**Hardening applied**: self-registration always creates a read-only `User`
account (the role is never client-supplied, so an anonymous caller cannot grant
itself Admin — Admin accounts are seeded or promoted by an existing admin); the
`/api/auth` endpoints are rate-limited (fixed window, 5 requests / 30 s) to blunt
brute-force and username enumeration; the login path runs a constant-time decoy
hash on unknown usernames so response timing does not reveal which accounts
exist; and startup fails fast in Production if `Jwt:Key` is missing, too short
for HMAC-SHA256, or still the committed development placeholder (supply it via
user-secrets or the `Jwt__Key` environment variable).

### 2.3 — Middleware concept

Middleware are components composed into a pipeline; each receives the
`HttpContext`, can act **before and after** invoking the next component, and
can short-circuit the chain (as the JWT middleware does when returning 401).
Registration order defines execution order — it is an onion: the first
registered component sees the request first and the response last.

The custom implementation is
[`RequestLoggingMiddleware`](src/Falck.Api/Middleware/RequestLoggingMiddleware.cs),
which logs method, path, caller IP, authenticated user, final status code and
elapsed time. It is registered outermost — before
`ExceptionHandlingMiddleware` — so it records the status code that error
handling actually produced. Sample output:

```
HTTP GET /api/employees from 127.0.0.1 as 'admin' => 200 in 244 ms
HTTP GET /api/employees/999 from 127.0.0.1 as 'admin' => 404 in 108 ms
```

### 5.1 — Common .NET performance issues and how to address them

- **Sync-over-async** (`.Result`, `.Wait()`, missing `async`): starves the
  thread pool under load. Use `async/await` end to end — every I/O path in this
  API is async and accepts a `CancellationToken`.
- **N+1 queries / lazy loading**: one query per row when iterating navigation
  properties. Use eager loading (`Include`) or projections (`Select` into
  DTOs) so EF issues one SQL statement.
- **Tracking read-only queries**: the change tracker costs memory and CPU. Use
  `AsNoTracking()` for reads (done in the list queries here).
- **Fetching more than needed**: `SELECT *` on wide tables, no pagination.
  Project only required columns and page (`Skip/Take`) large result sets.
- **Excessive allocations**: string concatenation in hot loops, large object
  heap churn. Use `StringBuilder`/`Span<T>`, pool buffers (`ArrayPool`), cache
  serializer instances.
- **No caching**: recomputing or re-reading immutable data per request. Use
  `IMemoryCache`/`HybridCache` or a distributed cache (Redis) with sensible
  invalidation.
- **Connection/socket misuse**: instantiating `HttpClient` per request causes
  socket exhaustion — use `IHttpClientFactory`. Let ADO.NET pool DB
  connections (don't hold them).
- **Middleware doing heavy work per request**: keep the hot path lean; move
  expensive work to background services or queues.

### 5.2 — Profiling and optimizing a slow query

1. **Measure first.** Enable EF Core logging (`LogTo` /
   `EnableSensitiveDataLogging` in dev) or interceptors to capture the exact
   SQL and its duration; at API level, the request logging middleware already
   exposes slow endpoints. In production, use Application Insights /
   OpenTelemetry traces or MiniProfiler to find the offending query instead of
   guessing.
2. **Reproduce and inspect the plan.** Run the captured SQL in SSMS with the
   actual execution plan (or `SET STATISTICS IO, TIME ON`). Look for scans on
   large tables, key lookups, implicit conversions and missing-index warnings.
3. **Fix the usual suspects, cheapest first:**
   - add/adjust indexes to match the `WHERE`/`JOIN`/`ORDER BY` (covering
     indexes where it pays off);
   - rewrite the LINQ so it translates well: filter/project in SQL, avoid
     client-side evaluation, avoid `Contains` over huge in-memory lists;
   - eliminate N+1 with `Include`/projections; use `AsNoTracking` for reads;
   - use pagination instead of materializing everything;
   - for hot read paths, consider a hand-written SQL query
     (`FromSqlInterpolated`) or a cached result.
4. **Verify.** Re-run with the same measurement (plan + timings, load test if
   the issue was concurrency) and keep the numbers — optimization without
   before/after data is folklore.

---

## Project structure

```
├── src/
│   ├── Falck.Domain/            # Entities, enums, bonus Strategy + Factory
│   ├── Falck.Application/       # Use cases, DTOs, ports (repositories, hasher, JWT)
│   ├── Falck.Infrastructure/    # EF Core, migrations, repositories, BCrypt, JWT
│   └── Falck.Api/               # Controllers, middleware, DI, Swagger
├── tests/
│   └── Falck.Tests/             # 25 unit tests (domain + auth service)
└── docs/
    └── database-schema.sql      # Full DDL exported from the migrations
```
