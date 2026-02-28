\# Library Management System — ASP.NET Core MVC



Sistema web de gestión de biblioteca desarrollado con \*\*ASP.NET Core MVC\*\*, enfocado en arquitectura backend, control de acceso seguro y procesamiento transaccional con manejo de inventario.



El proyecto está pensado para la gestión diaria de una biblioteca asumiendo empleados físicos en tienda, permisos de solo administrador, autogestión de clientes, partiendo de un crud básico se escaló usando enfoque empresarial especialmente en autenticación, autorización y manejo interno de operaciones.



---



\##  ¿Qué problema resuelve?



✔ Gestión multi-rol con acceso controlado a la información  

✔ Control de inventario separado para venta y préstamo  

✔ Procesamiento transaccional consistente entre múltiples libros  

✔ Validaciones financieras ejecutadas en servidor  

✔ Prevención de inconsistencias al trasladar validaciones críticas desde frontend hacia backend

---



\##  Enfoque técnico aplicado



\- Arquitectura MVC en capas

\- DTO Pattern para aislamiento de datos

\- Policy-Based Authorization

\- Claims-Based Authentication

\- Manejo de estados transaccionales

\- Service Layer para reglas de negocio

---



\##  Decisiones técnicas



\- Clean Layered Architecture

\- Claims-Based Authentication

\- Policy-Based Authorization

\- ACID Transaction Handling

\- Soft Delete Strategy

\- Service Layer Abstraction



---



\##  Architecture Overview



<details>

<summary>View Architecture Details</summary>



\### Layered Structure



\- \*\*Domain Models\*\*

\- \*\*DTO / ViewModels\*\*

\- \*\*Controllers\*\*

\- \*\*Service Layer\*\*

\- \*\*Data Access Layer (EF Core)\*\*



Concepts applied:



\- Separation of Concerns  

\- Dependency Injection  

\- Service-Oriented Design  

\- Layered Architecture  



DTOs prevent persistence model exposure to presentation layer.



</details>



---



\## Authentication \& Authorization



<details>

<summary>Security Implementation</summary>



\### Authentication

\- Cookie-Based Authentication

\- Claims-Based Identity

\- Custom Authentication Workflow

\- Session Persistence Control



\### Authorization

\- Policy-Based Authorization

\- Role-Based Access Control

\- Ownership Validation

\- Privilege Escalation Prevention

\- Application-Level Row Security



All authorization decisions are enforced in backend.



</details>



---



\##  Transaction \& Inventory Engine



<details>

<summary>Transaction System Details</summary>



Core transactional engine capable of processing multiple books per operation.



Implemented features:



\- Multi-item transactions

\- Atomic database operations

\- Real-time stock validation

\- Financial backend validation

\- Transaction number generation

\- Inventory synchronization



Business rules executed exclusively server-side.



</details>



---



\##  User Management



<details>

<summary>User Module</summary>



\- Role administration

\- Controlled profile editing

\- Secure registration

\- Soft delete accounts

\- Overposting protection



Security:

\- BCrypt password hashing

\- DTO-based input validation



</details>



---



\##  Catalog Management



<details>

<summary>Inventory Module</summary>



\- Book \& category administration

\- Separate stock handling (sale / loan)

\- Dynamic filtering

\- Historical consistency



Patterns used:

\- Query Composition

\- Relational Data Loading

\- Soft Delete Strategy



</details>



---



\##  Image Processing Service



<details>

<summary>Image Pipeline</summary>



Dedicated service layer for book images:



\- Automatic resizing

\- WebP compression

\- Unique filename generation

\- Safe resource replacement



Concepts:

\- File Storage Decoupling

\- Resource Lifecycle Management

\- Image Optimization Pipeline



</details>



---



\##  Data Modeling



<details>

<summary>Database Design</summary>



Built using \*\*Entity Framework Core (Code First)\*\*:



\- Relational entity modeling

\- Enum-based domain logic

\- Aggregate transaction modeling

\- Query-level filtering



Ensures full transactional traceability.



</details>



---



\##  Tech Stack



\### Backend

\- ASP.NET Core MVC

\- Entity Framework Core

\- LINQ

\- SQL Server



\### Architecture

\- MVC Pattern

\- DTO Pattern

\- Service Layer

\- Transaction Management



\### Security

\- Cookie Authentication

\- Claims Identity

\- Role-Based Access Control (RBAC)

\- Policy-Based Authorization

\- BCrypt Hashing



\### Frontend

\- Razor Views

\- Bootstrap



---



\## Learning Outcomes



Algunos de los aprendizajes más relevantes surgieron al enfrentar problemas como:



\- Cómo el model binding procesa automáticamente los datos enviados desde formularios y cómo atributos como `\[BindNever]` afectan directamente lo que el backend espera recibir en un POST.

\- Comprender que los modelos no solo representan datos, sino que también pueden encapsular reglas de negocio y validaciones mediante `IValidatableObject`.

\- La importancia de usar viewModel(DTO) para transefir solo los datos requeridos y esperados por el method.

\- La importancia de mover validaciones críticas desde controladores hacia el dominio para evitar inconsistencias.

\- Manejar correctamente estados transaccionales y sincronización de inventario cuando múltiples operaciones afectan una misma entidad usando el model desde el method.

\- Diferenciar entre hacer que una funcionalidad funcione y diseñarla para que sea segura y consistente según reglas del negocio.

