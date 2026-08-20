# Online Store API

A RESTful e-commerce backend built with ASP.NET Core, Clean Architecture, and SQL Server.

## Features

- Authentication & authorization
- Role- and permission-based access control
- Product and category management
- Product image management
- Customer management
- Order management
- Payment processing
- Order lifecycle management
- Business rule validation
- Pagination, filtering, and sorting

## Architecture

The project follows Clean Architecture principles with a clear separation of concerns:

- **API** — HTTP endpoints and request/response handling
- **Application** — Use cases, commands, queries, and application logic
- **Domain** — Entities, business rules, and domain logic
- **Infrastructure** — SQL Server persistence, repositories, and external services

## API

Interactive API documentation is available through Swagger/OpenAPI.

Swagger documents the available endpoints, request models, responses, authentication requirements, and status codes.

## Running the API

```bash
dotnet run --project OnlineStore.Api