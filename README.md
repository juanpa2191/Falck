# Falck — API de Gestión de Empleados

Prueba técnica para Desarrollador Backend .NET. API web ASP.NET Core 9 para
gestionar empleados, departamentos, proyectos e historial de cargos, protegida
con JWT y autorización basada en roles.

**Repositorio:** https://github.com/juanpa2191/Falck

## Stack tecnológico

- .NET 9 / ASP.NET Core Web API (controladores)
- Entity Framework Core 9 + SQL Server LocalDB
- Autenticación JWT Bearer, hashing de contraseñas con BCrypt
- Swagger (Swashbuckle) con soporte Bearer
- AutoMapper (perfiles separados por agregado) para el mapeo entidad → DTO
- xUnit (51 pruebas unitarias)
- Docker / docker-compose; CI/CD con GitHub Actions publicando en GHCR

## Puesta en marcha

### Requisitos previos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server LocalDB (viene con Visual Studio; verifícalo con `sqllocaldb info`).
  Para usar otra instancia de SQL Server, cambia
  `ConnectionStrings:DefaultConnection` en `src/Falck.Api/appsettings.json`.

### Ejecutar

```bash
git clone https://github.com/juanpa2191/Falck.git
cd Falck
dotnet run --project src/Falck.Api --launch-profile http
```

Eso es todo: al arrancar, la API aplica las migraciones de EF Core, crea la base
de datos `FalckDb` y siembra datos de demostración (departamentos,
proyectos, empleados con historial de cargos y dos cuentas de demo).

Abre **http://localhost:5229/swagger** para explorar la API.

### Cuentas de demostración

| Usuario  | Contraseña  | Rol   | Acceso                                    |
|----------|-------------|-------|-------------------------------------------|
| `admin`  | `Admin123!` | Admin | Todos los endpoints de empleados          |
| `user`   | `User123!`  | User  | Solo GET en los endpoints de empleados    |

Inicia sesión con `POST /api/auth/login`, copia el `token` de la respuesta y
úsalo con el botón **Authorize** en Swagger (o con la cabecera
`Authorization: Bearer <token>`). El archivo `src/Falck.Api/Falck.Api.http`
contiene peticiones de ejemplo listas para usar.

### Ejecutar con Docker (sin SQL Server local)

La ruta con `dotnet run` de arriba apunta a SQL Server **LocalDB** (solo
Windows). Para ejecutar en cualquier entorno, usa Docker Compose, que levanta un
contenedor de SQL Server para Linux junto a la API y los conecta entre sí:

```bash
docker compose up --build
```

Luego abre **http://localhost:8080/swagger**. Compose sobrescribe la cadena de
conexión para apuntar al servicio `db` y espera (mediante healthcheck) a que SQL
Server esté listo; la API entonces aplica las migraciones y siembra los mismos
datos de demo. Para detener: `docker compose down` (añade `-v` para borrar
también el volumen de la base de datos).

El `Dockerfile` es una construcción multi-etapa (SDK para publicar, runtime de
ASP.NET para ejecutar), y `Jwt__Key` / la cadena de conexión se pasan como
variables de entorno — el mismo mecanismo de sobrescritura que usaría un
despliegue real para los secretos.

### Pruebas

```bash
dotnet test
```

Cobertura (código escrito a mano, excluyendo las migraciones generadas):

```bash
dotnet test --settings coverlet.runsettings --collect:"XPlat Code Coverage"
```

## Integración y entrega continua (CI/CD)

Un único pipeline con **compuerta** en `.github/workflows/ci-cd.yml`, con dos
jobs encadenados: primero se prueban los cambios y **solo si pasan** se construye
o publica la imagen Docker (el job `build-image` declara `needs: test`).

| Disparador | Job `test` | Job `build-image` |
|---|---|---|
| **Pull request → `main`** | restaura, compila en Release con `-warnaserror` y corre las 51 pruebas con cobertura | construye la imagen **sin push** (valida el `Dockerfile`) |
| **Push a `main`** | igual | solo si `test` pasa → **publica** en GHCR `:latest` y `:sha-<corto>` |
| **Tag `v*.*.*`** | igual | solo si `test` pasa → **publica** en GHCR `:vX.Y.Z` |

Así `:latest` y los tags **siempre** corresponden a un commit que pasó las
pruebas; una regresión que compila pero rompe un test no llega al registro. En
los pull requests se valida que la imagen se construya, pero no se publica nada.
Se autentica con el `GITHUB_TOKEN` integrado, así que no se necesitan secretos
adicionales para publicar.

Descargar y ejecutar la imagen publicada (una vez que el workflow haya corrido
al menos una vez y el paquete se haya hecho público, o tras `docker login
ghcr.io`):

```bash
docker pull ghcr.io/juanpa2191/falck:latest
```

El pipeline produce un artefacto desplegable; apuntarlo a un host real (Azure
Web App for Containers, AWS ECS, una VM, etc.) es cuestión de añadir un paso de
despliegue con las credenciales de esa plataforma como secretos del repositorio.

## Endpoints

| Método | Ruta                                            | Roles       | Descripción |
|--------|-------------------------------------------------|-------------|-------------|
| POST   | `/api/auth/register`                            | anónimo     | Crea una cuenta User (solo lectura), devuelve un JWT |
| POST   | `/api/auth/login`                               | anónimo     | Autentica y devuelve un JWT con el claim de rol |
| GET    | `/api/employees`                                | Admin, User | Lista empleados (con bono anual calculado) |
| GET    | `/api/employees/{id}`                           | Admin, User | Detalle del empleado: historial de cargos + proyectos |
| POST   | `/api/employees`                                | Admin       | Crea empleado (abre su primer registro de historial) |
| PUT    | `/api/employees/{id}`                           | Admin       | Actualiza; un cambio de cargo se registra en el historial |
| DELETE | `/api/employees/{id}`                           | Admin       | Elimina empleado |
| GET    | `/api/departments`                              | Admin, User | Lista departamentos |
| GET    | `/api/departments/{id}/employees-with-projects` | Admin, User | Consulta de la sección 4.3 sobre HTTP |

## Arquitectura

**Clean Architecture** con la regla de dependencias apuntando hacia adentro:

```
Falck.Api ──────────► Falck.Application ──────► Falck.Domain
    │                        ▲
    └──► Falck.Infrastructure┘   (implementa los contratos de Application/Domain)
```

| Proyecto | Responsabilidad | Depende de |
|---|---|---|
| `Falck.Domain` | Entidades, reglas de negocio (estrategias de bono, regla de cambio de cargo) | nada |
| `Falck.Application` | Casos de uso (servicios), DTOs, contratos de repositorio/puertos | Domain |
| `Falck.Infrastructure` | EF Core, repositorios, BCrypt, generación de JWT | Application |
| `Falck.Api` | Controladores, middleware, composición de DI, Swagger | Application, Infrastructure |

La API nunca toca EF Core directamente; la capa de aplicación depende solo de
interfaces (`IEmployeeRepository`, `IPasswordHasher`, `IJwtTokenGenerator`) cuyas
implementaciones viven en Infrastructure.

### Patrones de diseño

1. **Strategy** — `IBonusStrategy` (`Falck.Domain/Strategies`) encapsula la
   política de bono: `RegularEmployeeBonusStrategy` (10%) y `ManagerBonusStrategy`
   (20%, compartida por todos los tipos de gerente). Se agregan nuevas políticas
   sin tocar `Employee` (Open/Closed).
2. **Factory** — `BonusStrategyFactory` resuelve la estrategia a partir del cargo
   del empleado, de modo que quien la invoca nunca ramifica según el cargo.
3. **Repository** — la persistencia se abstrae detrás de `IEmployeeRepository` /
   `IDepartmentRepository` / `IUserRepository`; las implementaciones con EF Core
   viven en `Falck.Infrastructure/Persistence/Repositories`. El `DbContext`
   cumple además el rol de Unit of Work.

### SOLID

- **S** — controladores delgados, un servicio de caso de uso por agregado, una
  estrategia por política de bono, un `IEntityTypeConfiguration` por entidad.
- **O** — nuevos tipos de gerente = una entrada de enum; nuevas políticas de bono
  = una nueva clase de estrategia. Nada existente cambia.
- **L** — toda `IBonusStrategy` es intercambiable donde se espera la interfaz; la
  factory se apoya en eso.
- **I** — contratos pequeños y enfocados (`IPasswordHasher` tiene 2 métodos) en
  lugar de un único "IRepository de todo".
- **D** — Application depende de abstracciones; EF Core, BCrypt y JWT son
  detalles de implementación inyectados desde las raíces de composición
  (`AddApplication()` / `AddInfrastructure()`).

### Decisiones de diseño a destacar

- **`CurrentPosition` es un enum respaldado por int** (`PositionType`). El
  enunciado pide `CurrentPosition (int)`; el enum persiste como `int` mientras
  mantiene los valores con seguridad de tipos. Los tipos de gerente ocupan una
  "banda gerencial" (valores >= 10), así que `IsManagerial()` cubre *muchos tipos
  de gerente* con una sola regla.
- **`PositionHistory.Position` es un string** exactamente como pide el enunciado;
  el valor es el nombre del `PositionType` para mantener consistencia con el enum.
- **Los cambios de cargo pasan por el dominio** — `Employee.ChangePosition()` es
  la única forma de cambiar de cargo: cierra el registro de historial abierto y
  abre uno nuevo. El servicio nunca manipula el historial a mano.
- **Tabla `Users` propia + BCrypt en lugar de ASP.NET Identity** — la prueba
  necesita dos roles y emisión de JWT; Identity completo agregaría 7 tablas y
  ocultaría lo interesante (hashing, claims) tras "magia".
- **Usuarios de demo sembrados al arrancar (`DbSeeder`), no vía `HasData`** —
  BCrypt emite un hash distinto por ejecución, lo que ensuciaría toda migración
  futura.
- **AutoMapper con un perfil por agregado** — el mapeo entidad → DTO vive en
  `EmployeeMappingProfile`, `DepartmentMappingProfile` y `ProjectMappingProfile`
  (uno por raíz de agregado, para respetar Responsabilidad Única). Los hijos de
  un agregado se mapean con su raíz: `PositionHistory` va en el perfil de Employee
  porque solo existe dentro de un empleado. Se registran con `AddAutoMapper` y los
  servicios dependen de `IMapper`. El bono anual sigue siendo lógica de dominio
  (`Employee.CalculateYearlyBonus` con la Strategy/Factory), invocada desde el
  perfil. Nota de seguridad: se fija AutoMapper 14 (última bajo licencia libre) y
  se suprime **solo** el advisory GHSA-rvv3-g6hj-g44x vía `NuGetAuditSuppress`
  —su parche está únicamente en las versiones comerciales 15.1.1+— porque el
  mapeo opera sobre tipos internos fijos, sin el vector de DoS reportado.
- **`nuget.config` local al repo** fijando nuget.org para que la solución se
  restaure de forma idéntica en cualquier máquina.

## Esquema de base de datos (sección 4.1)

Tablas: `Employees`, `Departments` (1:N), `Projects` (N:N vía
`EmployeeProjects`), `PositionHistories` (1:N, `EndDate NULL` = cargo actual),
`Users`. DDL completo:
[`docs/database-schema.sql`](docs/database-schema.sql). El salario es
`decimal(18,2)`; `PositionHistories` tiene un índice de cobertura sobre
`(EmployeeId, StartDate)`; `Users.Username` es único.

La consulta LINQ de la sección 4.3 vive en
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

## Respuestas escritas

### 2.2 — Cómo se implementan la autenticación y la autorización

**Autenticación** (quién eres): la API emite JWT sin estado. `POST
/api/auth/login` verifica el hash BCrypt y devuelve un token firmado con
HMAC-SHA256 que contiene `sub`, `unique_name`, `jti` y un claim de rol, con una
vigencia de 60 minutos. El middleware JWT bearer valida firma, emisor, audiencia
y vigencia en cada petición y construye el `ClaimsPrincipal`. La clave de firma
vive en la configuración para esta demo; en producción vendría de un almacén de
secretos (user-secrets, variables de entorno, Azure Key Vault) y los tokens se
combinarían con refresh tokens únicamente sobre HTTPS.

**Autorización** (qué puedes hacer): basada en roles, impulsada por el claim de
rol. `EmployeesController` está decorado con `[Authorize(Roles = "Admin,User")]`
y las acciones que modifican datos lo restringen a `[Authorize(Roles =
"Admin")]`, lo que produce exactamente la matriz de la sección 3.2: Admin =
acceso total, User = solo GET. Para reglas más finas, el mismo mecanismo escala a
autorización basada en políticas (`AddAuthorization(options =>
options.AddPolicy(...))`) con requisitos/handlers personalizados.

**Endurecimiento aplicado**: el auto-registro siempre crea una cuenta `User` de
solo lectura (el rol nunca lo suministra el cliente, así que un llamante anónimo
no puede otorgarse Admin — las cuentas Admin se siembran o las promueve un admin
existente); los endpoints `/api/auth` tienen límite de tasa (ventana fija, 5
peticiones / 30 s) para frenar fuerza bruta y enumeración de usuarios; el flujo
de login ejecuta un hash señuelo de tiempo constante ante usuarios desconocidos
para que el tiempo de respuesta no revele qué cuentas existen; y el arranque
falla rápido en Producción si `Jwt:Key` falta, es demasiado corta para
HMAC-SHA256, o sigue siendo el placeholder de desarrollo commiteado (suminístrala
vía user-secrets o la variable de entorno `Jwt__Key`).

### 2.3 — Concepto de middleware

El middleware son componentes compuestos en una tubería (pipeline); cada uno
recibe el `HttpContext`, puede actuar **antes y después** de invocar al siguiente
componente, y puede cortocircuitar la cadena (como hace el middleware JWT al
devolver 401). El orden de registro define el orden de ejecución — es una cebolla:
el primer componente registrado ve la petición primero y la respuesta al final.

La implementación personalizada es
[`RequestLoggingMiddleware`](src/Falck.Api/Middleware/RequestLoggingMiddleware.cs),
que registra método, ruta, IP del llamante, usuario autenticado, código de estado
final y tiempo transcurrido. Se registra en la capa más externa — antes de
`ExceptionHandlingMiddleware` — para que capture el código de estado que
realmente produjo el manejo de errores. Ejemplo de salida:

```
HTTP GET /api/employees from 127.0.0.1 as 'admin' => 200 in 244 ms
HTTP GET /api/employees/999 from 127.0.0.1 as 'admin' => 404 in 108 ms
```

### 5.1 — Problemas comunes de rendimiento en .NET y cómo abordarlos

- **Sync-over-async** (`.Result`, `.Wait()`, falta de `async`): agota el pool de
  hilos bajo carga. Usa `async/await` de extremo a extremo — cada ruta de I/O en
  esta API es asíncrona y acepta un `CancellationToken`.
- **Consultas N+1 / lazy loading**: una consulta por fila al iterar propiedades
  de navegación. Usa carga ansiosa (`Include`) o proyecciones (`Select` a DTOs)
  para que EF emita una sola sentencia SQL.
- **Tracking en consultas de solo lectura**: el change tracker cuesta memoria y
  CPU. Usa `AsNoTracking()` para lecturas (aplicado en las consultas de listado
  aquí).
- **Traer más de lo necesario**: `SELECT *` en tablas anchas, sin paginación.
  Proyecta solo las columnas requeridas y pagina (`Skip/Take`) los conjuntos de
  resultados grandes.
- **Asignaciones excesivas**: concatenación de strings en bucles calientes, churn
  en el Large Object Heap. Usa `StringBuilder`/`Span<T>`, agrupa buffers
  (`ArrayPool`), cachea instancias de serializadores.
- **Falta de caché**: recalcular o releer datos inmutables por petición. Usa
  `IMemoryCache`/`HybridCache` o una caché distribuida (Redis) con invalidación
  sensata.
- **Mal uso de conexiones/sockets**: instanciar `HttpClient` por petición causa
  agotamiento de sockets — usa `IHttpClientFactory`. Deja que ADO.NET agrupe las
  conexiones a la base de datos (no las retengas).
- **Middleware haciendo trabajo pesado por petición**: mantén el hot path ligero;
  mueve el trabajo costoso a servicios en segundo plano o colas.

### 5.2 — Perfilado y optimización de una consulta lenta

1. **Medir primero.** Habilita el logging de EF Core (`LogTo` /
   `EnableSensitiveDataLogging` en dev) o interceptores para capturar el SQL
   exacto y su duración; a nivel de API, el middleware de logging de peticiones ya
   expone los endpoints lentos. En producción, usa trazas de Application Insights
   / OpenTelemetry o MiniProfiler para encontrar la consulta culpable en lugar de
   adivinar.
2. **Reproducir e inspeccionar el plan.** Ejecuta el SQL capturado en SSMS con el
   plan de ejecución real (o `SET STATISTICS IO, TIME ON`). Busca scans en tablas
   grandes, key lookups, conversiones implícitas y advertencias de índices
   faltantes.
3. **Corregir los sospechosos habituales, del más barato primero:**
   - agrega/ajusta índices para que coincidan con el `WHERE`/`JOIN`/`ORDER BY`
     (índices de cobertura donde valga la pena);
   - reescribe el LINQ para que se traduzca bien: filtra/proyecta en SQL, evita
     evaluación del lado del cliente, evita `Contains` sobre listas enormes en
     memoria;
   - elimina el N+1 con `Include`/proyecciones; usa `AsNoTracking` para lecturas;
   - usa paginación en vez de materializar todo;
   - para hot paths de lectura, considera una consulta SQL escrita a mano
     (`FromSqlInterpolated`) o un resultado cacheado.
4. **Verificar.** Vuelve a ejecutar con la misma medición (plan + tiempos, prueba
   de carga si el problema era concurrencia) y conserva los números — optimizar
   sin datos de antes/después es folclore.

---

## Estructura del proyecto

```
├── src/
│   ├── Falck.Domain/            # Entidades, enums, Strategy + Factory de bono
│   ├── Falck.Application/       # Casos de uso, DTOs, puertos (repositorios, hasher, JWT)
│   ├── Falck.Infrastructure/    # EF Core, migraciones, repositorios, BCrypt, JWT
│   └── Falck.Api/               # Controladores, middleware, DI, Swagger
├── tests/
│   └── Falck.Tests/             # 51 pruebas unitarias (dominio + servicios + mapeo + infra)
└── docs/
    └── database-schema.sql      # DDL completo exportado desde las migraciones
```
