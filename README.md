# country-service
C# country web api with angular admin app.

Data access projects
- Interface and implementations for Sql Server, PostgreSql, MySql.
- Dynamic SQL for patch so that only 'dirty' columns are updated.
- Unit tests for data access projects.    

.Web Api
- Problem details (with added context), for general exceptions, validation failures and returned error object results such as BadRequest(...).
- Serilog with added request context, e.g. correlation id.
- IOptions to use configuration from the appsettings file in code. 
- Middleware that checks for a correlation id, if none, it creates a correlation id, passed on by header propagation.
- CORS.
- Health check at /healthz endpoint.
- Prometheus metrics at /metrics endpoint.
- OpenApi at /scalar endpoint. 
- Method handlers for Options, Head, Get, Post, Put, Patch and Delete HTTP methods.

Blazor web app
- Bootstrap css.
- Basic crud operations.
- Errors are sent to the server to be logged.


Razor pages website
- Bootstrap css.
- Some UI components
