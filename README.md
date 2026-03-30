# Project Management API
![.NET 8](https://img.shields.io/badge/.NET-8.0-blue)
![Architecture](https://img.shields.io/badge/Architecture-Clean_Architecture-green)
![CQRS](https://img.shields.io/badge/Pattern-CQRS-orange)
![Tests](https://img.shields.io/badge/Tests-Passed-brightgreen)

API REST diseñada con**.NET 8**, **Clean Architecture**, **CQRS** y **SQLite** para demostrar separación de responsabilidades, control de validación vs dominio y diseño orientado a testabilidad. 

Implementa un caso práctico típico de gestión de proyectos y tareas, aplicando patrones arquitectónicos reales utilizados en entornos profesionales.

---

## Como Ejecutar

### Prerrequisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalado.

### Ejecución


```bash
git clone https://github.com/dadeleac/project-management-api.git
cd project-management-api

# Restaurar dependencias
dotnet restore

# Ejecutar la API
dotnet run --project ProjectManagement.Api
```

La aplicacion:
- Aplica migraciones automaticamente al iniciar (`MigrateAsync`).
- Crea la base de datos SQLite en `data/ProjectManagement.db`.
- Ejecuta un seed con datos de ejemplo si la base de datos esta vacia.
- Swagger disponible en `http://localhost:5070/swagger` (modo Development).
- Tests: `dotnet test` — 36 tests (xUnit + Moq + FluentAssertions + SQLite in-memory).

### Ejecución Tests
```bash
dotnet test
```

---

## Estructura del proyecto

```
ProjectManagement.slnx
├── ProjectManagement.Api            → Host, Controllers, Middleware, Request Models
├── ProjectManagement.Application    → Commands, Queries, Validators, DTOs, Interfaces
├── ProjectManagement.Domain         → Entities, Enums, Errors, Exceptions, Constraints
├── ProjectManagement.Infrastructure → DbContext, Repositories, Query Services, Migrations
└── ProjectManagement.Tests          → Unit + Integration tests
```

Las dependencias fluyen hacia adentro: **Api → Application → Domain ← Infrastructure**. Domain no referencia a ningun otro proyecto. Infrastructure implementa las interfaces definidas en Application (Dependency Inversion).

---

## Decisiones de arquitectura

### Diseño
Se ha aplicado **Clean Architecture** con un enfoque pragmático, evitando capas innecesarias en favor de un flujo directo basado en MediatR.


### CQRS con MediatR — Separacion explícita de lectura y escritura

Los Commands mutan estado a traves de **Repositories** (`IProjectRepository`, `ITaskItemRepository`), mientras que las Queries leen a traves de **Query Services** (`IProjectQueryService`, `ITaskQueryService`). 

Esta separacion no es accidental: permite que las lecturas usen proyecciones optimizadas (DTOs directos desde EF, sin cargar navegaciones innecesarias) mientras que las escrituras trabajan con entidades de dominio completas.

Las interfaces estan separadas fisicamente en `Application/Common/Interfaces/Commands/` y `Queries/` para que la separacion sea estructural, no solo nominal.

### Request Models en API separados de Commands

Los controllers reciben `CreateProjectRequest`, `CreateTaskRequest`, etc., y los mapean internamente al Command correspondiente. La razon principal: en endpoints como `POST /api/projects/{projectId}/tasks`, el `projectId` viene de la ruta y el body solo contiene titulo, descripcion, prioridad y fecha. 

Si el Command fuera el modelo de binding directo, tendriamos que duplicar el ID o aceptar inconsistencias entre ruta y body. Al separar, el controller es responsable de componer el Command con datos de multiples origenes (ruta + body).


### ValidationBehavior como pipeline de MediatR

Registrado como `IPipelineBehavior<TRequest, TResponse>` abierto, intercepta **todos** los requests que tengan un `IValidator<T>` registrado. Si hay errores, lanza `RequestValidationException` *antes* de que el handler se ejecute. Esto centraliza la validacion en un unico punto: los handlers asumen que los datos que reciben ya son validos y se enfocan exclusivamente en reglas de negocio.

FluentValidation se registra por assembly scan (`AddValidatorsFromAssembly`), asi que agregar un nuevo validator es solo crear la clase sin que haya que tocar el DI manualmente.


### Abstracción de Persistencia: el uso intencional de Repositorios
Para un proyecto de esta escala, llamar directamente al contexto desde la capa de Application sería una solución pragmática válida, haciendo uso de `DbContext` como Unit of Work y `DbSet` como el patrón Repositorio. 

 Sin embargo, se ha optado por una capa de Repositorio explícita por tres razones estratégicas:

**1. Blindaje del Dominio**: Al usar IProjectRepository, la capa de Aplicación no sabe nada de EF Core. Esto evita que "fugas" de lógica de infraestructura (como métodos específicos de LINQ to Entities o estados de seguimiento de EF) contaminen los Handlers.

**2. Testabilidad Pura**: Aunque EF Core permite InMemoryDatabase, el uso de interfaces permite realizar Mocking total con Moq en los tests unitarios. Esto garantiza que los tests de los Handlers se centren en la lógica de orquestación y no en el comportamiento del motor de base de datos.

**3. Contratos Semánticos**: En lugar de un `DbSet.Add()`, el dominio habla en términos de negocio: `AddAsync()`. Esto permite, por ejemplo, centralizar la lógica de Soft Delete o la auditoría en un solo punto del repositorio sin repetir código en cada Handler.

### Errores de dominio con claves (`Error(Code, MessageKey)`) + traductor en API

El dominio emite errores con claves inmutables (`"project.has_in_progress_tasks.message"`). La capa API traduce esas claves a mensajes legibles mediante `ErrorMessageTranslator`. Esto tiene dos ventajas concretas:

1. **El dominio no depende de ningun idioma**: cambiar mensajes o agregar idiomas no requiere tocar Domain ni Application.
2. **Los codigos de error son estables para clientes**: un frontend puede hacer `switch` sobre el codigo sin que un cambio de redaccion rompa la logica.

### Soft Delete con `HasQueryFilter` global

El borrado logico se implementa con un global query filter en EF Core (`HasQueryFilter(t => !t.IsDeleted)`). Todas las queries filtran automaticamente las tareas eliminadas sin que los handlers o query services necesiten recordarlo. 

Para la idempotencia del `DELETE` (RN-04), el repositorio usa `IgnoreQueryFilters()` para localizar la tarea incluso si ya fue borrada, y siempre retorna 204.

### `PagedQuery<T>` abstracto

Todas las queries paginadas heredan de `PagedQuery<T>`, que normaliza `PageNumber` (minimo 1) y `PageSize` (minimo 1, maximo 50) y calcula `Skip` automaticamente. Esto evita repetir logica de limites en cada handler y garantiza que RN-06 se cumpla uniformemente.

### FluentAssertions: Tests como documentación
Se ha optado por utilizar FluentAssertions en lugar de las aserciones nativas de xUnit. Esta decisión, aunque añade una dependencia adicional, se justifica por:

- **Legibilidad Natural**: Permite escribir tests que se leen como frases en inglés (result.Should().BeEquivalentTo(expected)), lo que facilita que cualquier desarrollador (o incluso un perfil menos técnico) entienda el objetivo de la prueba.

- **Mensajes de Error Descriptivos**: Ante un fallo, FluentAssertions proporciona detalles claros sobre qué falló y por qué (comparación de propiedades, diferencias en colecciones, etc.), reduciendo drásticamente el tiempo de depuración.

- **Encadenamiento de Reglas**: Permite validar múltiples condiciones en una sola línea de forma semántica, mejorando la densidad y claridad del código de test.

### Middleware global de errores

Un unico `ExceptionHandlingMiddleware` mapea excepciones a respuestas HTTP con formato consistente:

| Excepcion | HTTP Status | Cuando |
|---|---|---|
| `RequestValidationException` | 400 | Errores de input: FluentValidation falla (pipeline) |
| `NotFoundException` | 404 | Entidad no existe |
| `DomainException` | 422 | Violación de lógica de negocio (ej. RN-01, RN-02) |
| `Unhandled Exception` | 500 | Error no controlado (se registra log con stack trace) |

Los handlers no necesitan try/catch ni devolver `IActionResult` condicional. Lanzan excepciones y el middleware se encarga. 

### Logging con Serilog

Configurado via `appsettings.json` con sinks a consola y archivo JSON con rotacion diaria.

 `UseSerilogRequestLogging()` registra automaticamente cada request HTTP.

---

## Endpoints
### Projects
| Metodo | Ruta | Descripcion |
|---|---|---|
| `POST` | `/api/projects` | Crear proyecto |
| `GET` | `/api/projects` | Listar proyectos (paginado, filtro por status) |
| `GET` | `/api/projects/{id}` | Detalle con resumen estadistico |
| `PATCH` | `/api/projects/{id}/archive` | Archivar proyecto |
| `POST` | `/api/projects/{projectId}/tasks` | Crear tarea en proyecto |
| `GET` | `/api/projects/{projectId}/tasks` | Listar tareas (paginado, filtro status/priority) |

### Tasks
| Metodo | Ruta | Descripcion |
|---|---|---|
| `GET` | `/api/tasks/{id}` | Detalle de tarea |
| `PATCH` | `/api/tasks/{id}/status` | Cambiar estado de tarea |
| `DELETE` | `/api/tasks/{id}` | Eliminar tarea (soft delete) |

## Reglas de negocio

Implementadas en los **Command Handlers**, no en controllers ni en validators. Los validators solo validan formato y tipos, mientras que la logica de negocio vive en Application porque necesita acceso a datos (verificar estado del proyecto, contar tareas en progreso).

| RN | Regla | Donde |
|---|---|---|
| RN-01 | No se pueden crear tareas en un proyecto archivado | `CreateTaskItemCommandHandler` verifica `project.Status` |
| RN-02 | No se puede archivar un proyecto con tareas `InProgress` | `ArchiveProjectCommandHandler` consulta el repositorio |
| RN-03 | Al pasar a `Done`, se asigna `CompletedAt = DateTime.UtcNow` | `TaskItem.UpdateStatus()` en la entidad de dominio |
| RN-04 | Soft delete cumple con el estandar HTTP donde `DELETE` debe ser idempotente: devuelve `204` | `TaskItemRepository` usa `IgnoreQueryFilters()` |
| RN-05 | `GET /api/projects/{id}` incluye resumen estadistico | `ProjectQueryService` calcula conteos por estado |
| RN-06 | Paginacion obligatoria con defaults (page=1, size=10, max=50) | `PagedQuery<T>` normaliza parametros |

## Validacion (FluentValidation)

Cada Command tiene su propio Validator registrado automaticamente. Ejemplos de reglas:

- **CreateProjectCommand**: `Name` obligatorio (max 80 chars), `OwnerId` no vacio.
- **CreateTaskItemCommand**: `ProjectId` no vacio, `Title` obligatorio (max 100 chars), `Priority` debe ser enum valido, `DueDate` no puede estar en el pasado.
- **ArchiveProjectCommand**: `ProjectId` no vacio.
- **UpdateTaskItemStatusCommand**: `TaskItemId` no vacio, `NewStatus` debe ser enum valido.
- **DeleteTaskItemCommand**: `TaskItemId` no vacio.

La ejecucion es transparente via `ValidationBehavior<TRequest, TResponse>` en el pipeline de MediatR.

---

## Testing

36 tests distribuidos en las siguientes categorias:

| Categoria | Tests | Que validan |
|---|---|---|
| **Domain** | `ProjectTests`, `TaskItemTests` | Construccion de entidades, invariantes (trim, longitud), `Archive()`, `UpdateStatus()`, `MarkAsDeleted()` |
| **Command Handlers** | `ArchiveProjectCommandHandlerTests`, `CreateTaskItemCommandHandlerTests` | RN-01, RN-02, NotFoundException, happy path |
| **Query Handlers** | `GetProjectsQueryHandlerTests`, `GetTasksQueryHandlerTests` | Mapeo correcto a DTOs, paginacion |
| **Validators** | `CreateProjectCommandValidatorTests`, `CreateTaskItemCommandValidatorTests` | Campos vacios, longitud maxima, enum invalido, fecha pasada |
| **Integracion** | `ProjectQueryServiceTests`, `TaskItemRepositoryTests` | SQLite in-memory real: paginacion, soft delete, query filters |

Los tests de integracion usan SQLite in-memory para validar que los query filters de EF Core y la paginacion funcionan contra un motor de base de datos real, no contra mocks. 

Los tests unitarios usan Moq para aislar la logica de los handlers.
```bash
dotnet test
```
---

## Archivo `.http`

`ProjectManagement.Api/ProjectManagement.Api.http` contiene flujos de prueba manual ejecutables desde VS/Rider:

1. **Happy path**: crear proyecto → crear tarea → listar → cambiar estado → verificar estadisticas
2. **Reglas de negocio**: archivar con tareas en curso (422), crear tarea en proyecto archivado (422)
3. **Edge cases**: fecha pasada (400), soft delete idempotente (204 repetido), limite de paginacion (pedir 100 → recibir 50), 404

---

## Supuestos y decisiones adicionales

- **`OwnerId` como FK logica**: no existe modulo de autenticacion, pero se asume que el ID del propietario se validara en un sistema externo.
- **Fechas en UTC**: todas las fechas (`CreatedAt`, `CompletedAt`, `DueDate`) se manejan en UTC para evitar ambiguedades. En nuevas versiones de .NET se recomienda migrar a `TimeProvider`.
- **Enums como strings**: tanto en JSON (`JsonStringEnumConverter`) como en la base de datos, para legibilidad en queries directas y respuestas de API.
- **Seed condicional**: `ApplicationDbContextSeed` solo inserta datos si la base esta vacia, para no duplicar en reinicios.
- **Entidad `TaskItem` (no `Task`)**: evita colision con `System.Threading.Tasks.Task` — la tabla en BD se llama `Tasks` y la ruta se ha modificado para que sea `/api/tasks`.
