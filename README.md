# BRELTV - Business Rule Engine for Loan-to-Value

A .NET Core 8 API for evaluating loan applications based on customer profiles, income proof, and other criteria to determine Loan-to-Value (LTV) ratios and Field Investigation (FI) requirements.

## Features

- Dynamic Business Rule Engine (BRE) for loan evaluation
- Data-driven rules with approval workflow
- Customer profile management
- Loan evaluation history tracking
- Comprehensive API for rule management
- Docker support for easy deployment

## Architecture

The solution follows Clean Architecture principles with the following projects:

- **BRELTV.API**: REST endpoints and configuration
- **BRELTV.Application**: Business logic and CQRS pattern implementation
- **BRELTV.Domain**: Core entities and interfaces
- **BRELTV.Infrastructure**: Data access layer and external services
- **BRELTV.UnitTests**: Unit tests for the application
- **BRELTV.IntegrationTests**: Integration tests for the API

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (or Docker for containerized deployment)
- Visual Studio 2022 or VS Code

### Running Locally

1. Clone the repository
2. Update the connection string in `appsettings.json` if needed
3. Run the following commands:

```bash
dotnet restore
dotnet build
dotnet run --project src/BRELTV.API/BRELTV.API.csproj
```

### Using Docker

```bash
docker-compose up -d
```

This will start the API and a SQL Server instance with the correct configuration.

## API Endpoints

### Loan Evaluation

- `POST /api/LoanEvaluations` - Evaluate a loan application

### Business Rules Management

- `GET /api/BusinessRules` - Get all business rules
- `GET /api/BusinessRules/{id}` - Get a specific business rule
- `POST /api/BusinessRules` - Create a new business rule
- `GET /api/BusinessRules/pending-approvals` - Get pending rule approvals
- `POST /api/BusinessRules/approve/{id}` - Approve a business rule
- `POST /api/BusinessRules/reject/{id}` - Reject a business rule

## Database Structure

The database includes the following main tables:

- **CustomerProfiles**: Stores credit score bands and their descriptions
- **BusinessRules**: Contains LTV rules and FI requirements for each profile
- **RuleApprovals**: Tracks approval workflow for rule changes
- **LoanEvaluations**: Records evaluation history for auditing

## Business Rule Logic

The core logic evaluates loan applications based on:

1. Customer credit profile (0, -1, 650-700, 700-750, 750+)
2. Income proof availability (Yes/No)
3. Income proof amount (must be >= 25,000)
4. Floating money percentage after expenses and EMI (must be > 50%)

The output includes:
- LTV (Loan-to-Value) percentage
- FI Requirement (Mandatory or Waiver)
- Reason for the decision (for audit purposes)

## Testing

Run the tests using:

```bash
dotnet test
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.

