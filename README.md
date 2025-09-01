# country-service
C# Country Web Api with Blazor admin app.

Data access projects
- Interface and implementations for Sql Server, PostgreSql, MySql.
- Dynamic SQL for patch so that only 'dirty' columns are updated.
- Unit tests for data access projects.    

Web api project
- Problem details (with added context), for general exceptions, validation failures and logical errors such as duplication.
- Serilog with added request context, e.g. correlation id.
- IOptionsMonitor to use configuration from the appsettings file in code. 
- Middleware that checks for a correlation id, if none, it creates a correlation id, passed on by header propagation.
- CORS.
- Health check at /healthz endpoint.
- Prometheus metrics at /metrics endpoint.
- OpenApi at /scalar endpoint. 
- Request handlers for OPTIONS, HEAD, GET, POST, PUT, PATCH and DELETE HTTP methods.

Dtos, models and mappers projects
- Dtos give independence from the Database table design, shared project between Web Api and Blazor app. 
- Models hold database data.
- Mapper map between Dtos and Models. 

Shared project
- Code shared project between Web Api and Blazor app.

Blazor web app project
- Bootstrap css.
- CRUD operations.
- Client side validation with Data annotations.
- Toast notifications.
- Custom grid component with paging, sorting and filtering.
- Uses BlazorBootstrap component library.
- Errors are sent to the server to be logged.

Razor pages website
- Not worked on yet.
