# BRELTV - Business Rule Engine for Loan-to-Value Evaluation

A .NET Core 8 API for evaluating loan applications based on business rules.

## Overview

This project implements a Business Rule Engine (BRE) that evaluates loan applications based on customer profiles, income proof, and other factors to determine the appropriate Loan-to-Value (LTV) ratio and Field Investigation (FI) requirements.

## Features

- Dynamic business rules configuration
- Customer profile management
- Loan evaluation based on configurable rules
- Two-step approval workflow for rule changes
- Comprehensive error handling
- Audit trail for all evaluations and rule changes
- RESTful API with Swagger documentation
- Docker support for easy deployment

## Project Structure

The solution follows a simplified microservice architecture with the following projects:

- **BRELTV.API**: API controllers, middleware, and validators
- **BRELTV.Services**: Business logic and service implementations
- **BRELTV.DataAccess**: Data access layer using ADO.NET
- **BRELTV.Models**: Entity models, DTOs, and exceptions
- **BRELTV.Tests**: Unit and integration tests

## Business Rules

The BRE evaluates loan applications based on:

1. Customer Profile (Credit score bands: 0, -1, 650-700, 700-750, 750+)
2. Income Proof Availability (Yes/No)
3. Income Proof Amount (Minimum ₹25,000)
4. Floating Money After Expenses and EMI (>50%)

Outputs:
- LTV (Loan-to-Value) percentage
- FI (Field Investigation) requirement
- Reason for the decision

## Database Schema

The database includes the following tables:

1. **CustomerProfiles**: Stores customer profile bands and their descriptions
2. **BusinessRules**: Stores LTV and FI rules for each customer profile
3. **LoanEvaluations**: Records all loan evaluations performed
4. **RuleApprovals**: Tracks the approval workflow for business rule changes

## API Endpoints

### Loan Evaluation

- `POST /api/LoanEvaluations`: Evaluates a loan application and returns LTV and FI requirements

### Business Rules

- `GET /api/BusinessRules`: Gets all business rules
- `GET /api/BusinessRules/{id}`: Gets a business rule by ID
- `POST /api/BusinessRules`: Creates a new business rule
- `GET /api/BusinessRules/pending-approvals`: Gets pending business rule approvals
- `POST /api/BusinessRules/approve/{id}`: Approves a business rule
- `POST /api/BusinessRules/reject/{id}`: Rejects a business rule

### Customer Profiles

- `GET /api/CustomerProfiles`: Gets all customer profiles
- `GET /api/CustomerProfiles/{id}`: Gets a customer profile by ID
- `GET /api/CustomerProfiles/band/{profileBand}`: Gets a customer profile by band
- `POST /api/CustomerProfiles`: Creates a new customer profile
- `PUT /api/CustomerProfiles/{id}`: Updates an existing customer profile

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (or Docker)
- Docker (optional)

### Running with Docker

1. Clone the repository
2. Run `docker-compose up -d`
3. Access the API at http://localhost:8080/swagger

### Running Locally

1. Clone the repository
2. Update the connection string in `appsettings.json`
3. Run the database script in `src/Database/schema.sql`
4. Run the API project: `dotnet run --project src/BRELTV.API/BRELTV.API.csproj`
5. Access the API at https://localhost:5001/swagger

## Testing

Run the tests using:

```bash
dotnet test src/BRELTV.Tests/BRELTV.Tests.csproj
```

