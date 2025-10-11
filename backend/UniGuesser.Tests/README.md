# UniGuesser.Tests

This project contains unit tests for the UniGuesser backend application.

## Test Framework

- **xUnit**: Testing framework
- **Moq**: Mocking library
- **Entity Framework Core InMemory**: In-memory database provider for testing

## Test Coverage

### Services
- **DistanceCalculatorTests**: Tests for the distance calculation service (6 tests)
  - Same coordinates
  - Warsaw to Berlin distance
  - New York to London distance
  - Equator points
  - Opposite hemispheres
  - Small distance calculations

### Repositories
- **PlacesRepositoryTests**: Tests for the Places repository (11 tests)
  - Create operations
  - Get operations (by ID and by PublicId)
  - GetAll operations
  - Delete operations
  - Update operations
  - GetPlacesCount
  - GetPlacesByDifficulty

## Running Tests

From the `backend` directory:

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity detailed

# List all available tests
dotnet test --list-tests

# Run specific test class
dotnet test --filter "FullyQualifiedName~DistanceCalculatorTests"
```

## Test Structure

Tests are organized by the component they test:
- `Services/` - Tests for service classes
- `Repositories/` - Tests for repository classes

All tests follow the Arrange-Act-Assert pattern for clarity and consistency.
