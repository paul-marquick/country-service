# country-service
C# country web api with angular admin app.

Data access projects

- Interface and several implementations for differnet DBS such as PostgreSql, MySql, Sql Server.
- Unit tests for data access projects.
    

.NET web api

- Problem details (with added context), for general exceptions, validation failures and returned error object results such as BadRequest(...).
- Serilog with added request context, e.g. correlation id.
- Middleware that checks for a correlation id, if none, it creates a correlation id, passed on by header propagation.
- CORS.

Angular web app

- Basic crud operations. 
- Custom sync validator.
- Custom async validator.
- Environment config files.
