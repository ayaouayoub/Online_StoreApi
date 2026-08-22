# Online Store API

A production-style RESTful e-commerce backend built with **ASP.NET Core Web API, C#, Clean Architecture, and SQL Server**.

The API manages the core e-commerce workflow from product and customer management to order creation, payment processing, and order lifecycle management, while enforcing business rules, authentication, authorization, and data consistency.

## Features

* Authentication & authorization
* JWT-based authentication
* Role- and permission-based access control
* User and customer management
* Product and category management
* Product image upload and management
* Order management
* Order item management
* Payment processing
* Order lifecycle management
* Business rule validation
* Pagination, filtering, and sorting
* Global error handling
* Swagger/OpenAPI documentation

## Business Rules

The API enforces business rules to ensure that invalid operations are rejected before affecting the system.

Examples include:

* Validate product availability before adding items to an order.
* Prevent invalid order quantities and product selections.
* Validate payment amounts against the order total.
* Enforce valid order status transitions throughout the order lifecycle.
* Prevent unauthorized users from performing protected operations.
* Enforce role and permission requirements for administrative operations.
* Validate product and category relationships.
* Maintain data consistency during operations involving orders and payments.

## Architecture

The project follows **Clean Architecture** with a clear separation of concerns:

* **API** - HTTP endpoints, authentication, authorization, middleware, and request/response handling.
* **Application** - Use cases, commands, queries, handlers, DTOs, validation, and application logic.
* **Domain** - Entities, business rules, domain logic, enums, and domain exceptions.
* **Infrastructure** - SQL Server persistence, repositories, ADO.NET implementations, and external services.

This architecture keeps business logic independent from the API and infrastructure layers, making the system easier to maintain, test, and extend.

## Security

The API implements:

* JWT authentication
* Role-based authorization
* Permission-based authorization
* Claims-based authorization
* Protected administrative operations
* Active-user validation

## Database

The application uses **SQL Server** for relational data persistence.

Database operations include:

* Products
* Categories
* Product images
* Users
* Roles
* Permissions
* Customers
* Orders
* Order items
* Payments

Data access is implemented using **ADO.NET**, SQL queries, stored procedures, and transactional operations where required.

## API

Interactive API documentation is available through **Swagger/OpenAPI**.

Swagger documents the available endpoints, request models, responses, authentication requirements, and HTTP status codes.

## Running the API

```bash
dotnet run --project OnlineStore.Api
```

Once the API is running, Swagger can be used to explore and test the available endpoints.

## Technologies

* C#
* .NET 8
* ASP.NET Core Web API
* SQL Server
* T-SQL
* ADO.NET
* Clean Architecture
* CQRS / MediatR
* JWT
* Swagger / OpenAPI
* Git / GitHub
