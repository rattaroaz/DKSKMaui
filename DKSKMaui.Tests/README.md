# DKSKMaui Test Framework

## 📋 Overview

Comprehensive test suite for the DKSKMaui painting contractor management application. This test framework provides unit tests, integration tests, and component tests with full code coverage reporting.

## 🚀 Quick Start

### Run All Tests with Coverage
```powershell
# Windows PowerShell
.\run-tests.ps1 -OpenReport

# Or use the batch file (double-click or run from cmd)
run-tests.bat
```

### Run Specific Tests
```bash
# Run only unit tests
dotnet test --filter "Category=Unit"

# Run tests for a specific service
dotnet test --filter "FullyQualifiedName~CompanyService"

# Run with verbosity
dotnet test --logger "console;verbosity=detailed"
```

## 🧪 Test Architecture

### Test Categories

#### **Unit Tests** (Services & Models)
- `CompanyServiceTests` - Tests for company management service
- `InvoiceServiceTests` - Tests for invoice operations
- `ContractorServiceTests` - Tests for contractor management
- `InvoiceTests` - Invoice model validation
- `CompanyTests` - Company model validation  
- `ContractorTests` - Contractor model validation

#### **Integration Tests**
- `InvoiceWorkflowTests` - End-to-end invoice workflow testing
  - Invoice creation to payment completion
  - Multi-invoice management
  - Contractor payroll calculations
  - Aging report categorization
  - Property unit tracking
  - Transaction rollback testing

#### **Component Tests** (Blazor UI)
- `HomePageTests` - Home page component testing
- `CreateInvoiceTests` - Invoice creation UI testing

### Infrastructure

- **TestBase** - Base class for unit tests with in-memory database
- **BlazorTestBase** - Base class for Blazor component tests using bUnit
- **TestDataBuilder** - Fluent test data generation using Bogus
- **MockNavigationManager** - Navigation testing support

## 📊 Code Coverage

### Current Coverage Goals
- **Line Coverage**: 70% minimum
- **Branch Coverage**: 70% minimum  
- **Method Coverage**: 70% minimum
- **Critical Path Coverage**: 90% target

### Coverage Configuration
Coverage settings are defined in:
- `DKSKMaui.Tests.csproj` - MSBuild coverage settings
- `coverlet.runsettings` - Detailed coverage configuration

### Exclusions
- Migration files
- Auto-generated code
- Designer files
- Program/MauiProgram classes
- Test assemblies

## 🛠️ Technology Stack

| Package | Version | Purpose |
|---------|---------|---------|
| xUnit | 2.8.1 | Testing framework |
| Moq | 4.20.70 | Mocking framework |
| FluentAssertions | 6.12.0 | Assertion library |
| bUnit | 1.24.10 | Blazor component testing |
| Coverlet | 6.0.2 | Code coverage |
| ReportGenerator | 5.3.8 | Coverage reporting |
| Bogus | 35.5.0 | Test data generation |
| AutoFixture | 4.18.1 | Test fixture creation |
| EF Core InMemory | 8.0.8 | In-memory database testing |

## 📁 Project Structure

```
DKSKMaui.Tests/
├── Infrastructure/          # Test base classes and utilities
│   ├── TestBase.cs         # Base class with DB setup
│   ├── BlazorTestBase.cs   # Blazor component test base
│   └── TestDataBuilder.cs  # Test data factory
├── Models/                  # Model validation tests
│   ├── InvoiceTests.cs
│   ├── CompanyTests.cs
│   └── ContractorTests.cs
├── Services/                # Service layer tests
│   ├── CompanyServiceTests.cs
│   ├── InvoiceServiceTests.cs
│   └── ContractorServiceTests.cs
├── Components/              # Blazor component tests
│   └── Pages/
│       ├── HomePageTests.cs
│       └── CreateInvoiceTests.cs
├── Integration/             # Integration tests
│   └── InvoiceWorkflowTests.cs
├── TestResults/            # Test outputs (gitignored)
├── coverlet.runsettings    # Coverage configuration
├── run-tests.ps1           # PowerShell test runner
├── run-tests.bat           # Windows batch runner
└── DKSKMaui.Tests.csproj  # Test project file
```

## 🔧 Writing New Tests

### Unit Test Example
```csharp
public class MyServiceTests : TestBase
{
    private readonly MyService _service;
    
    public MyServiceTests()
    {
        _service = ServiceProvider.GetRequiredService<MyService>();
    }
    
    [Fact]
    public async Task MyMethod_ShouldReturnExpectedResult()
    {
        // Arrange
        var testData = TestDataBuilder.CreateMyEntity();
        
        // Act
        var result = await _service.MyMethodAsync(testData);
        
        // Assert
        result.Should().NotBeNull();
        result.Property.Should().Be(expectedValue);
    }
}
```

### Component Test Example
```csharp
public class MyComponentTests : BlazorTestBase
{
    [Fact]
    public void MyComponent_RendersCorrectly()
    {
        // Act
        var component = RenderComponent<MyComponent>();
        
        // Assert
        component.Find("h1").TextContent.Should().Be("Expected Title");
    }
}
```

### Integration Test Example
```csharp
[Fact]
public async Task CompleteWorkflow_Success()
{
    // Arrange
    var entity = TestDataBuilder.CreateEntity();
    
    // Act - Multiple service interactions
    await _service1.CreateAsync(entity);
    await _service2.ProcessAsync(entity.Id);
    
    // Assert
    var result = await _service3.GetResultAsync(entity.Id);
    result.Status.Should().Be(ExpectedStatus);
}
```

## 🐛 Debugging Tests

### Visual Studio
1. Open Test Explorer (Test → Test Explorer)
2. Right-click on test → Debug
3. Set breakpoints in test or production code
4. Use immediate window for inspection

### VS Code
1. Install C# extension
2. Use `.NET Core Test Explorer` extension
3. Click on "Debug Test" above test methods
4. Use Debug Console for inspection

### Command Line
```bash
# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run with diagnostic logs
dotnet test --diag:log.txt

# Run specific test with logs
dotnet test --filter "FullyQualifiedName=Namespace.ClassName.MethodName" --logger "trx;LogFileName=test.trx"
```

## 📈 Coverage Reports

### Generate Coverage Report
```powershell
# Generate and open HTML report
.\run-tests.ps1 -OpenReport

# Generate without opening
.\run-tests.ps1
```

### Report Location
- HTML Report: `./TestResults/CoverageReport/index.html`
- Cobertura XML: `./TestResults/coverage.cobertura.xml`
- LCOV: `./TestResults/coverage.info`

### Coverage Badges
After running tests, coverage badges are generated in:
`./TestResults/CoverageReport/badge_*.svg`

## ✅ Best Practices

1. **Test Isolation** - Each test uses its own database instance
2. **AAA Pattern** - Arrange, Act, Assert structure
3. **Descriptive Names** - Test names describe what they test
4. **Test Data Builders** - Use TestDataBuilder for consistent data
5. **Mocking** - Mock external dependencies
6. **Async Testing** - Properly await async operations
7. **Fluent Assertions** - Use FluentAssertions for readable tests
8. **Category Attributes** - Tag tests with appropriate categories

## 🚨 Common Issues & Solutions

### Issue: Tests fail with "database already exists"
**Solution**: Tests use unique database names. Clean and rebuild if issue persists.

### Issue: Coverage report not generating
**Solution**: Ensure ReportGenerator is installed:
```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
```

### Issue: Component tests fail with service not found
**Solution**: Register required services in BlazorTestBase.ConfigureServices()

### Issue: In-memory database doesn't support specific feature
**Solution**: Use SQLite in-memory mode for more complete SQL support:
```csharp
options.UseSqlite("DataSource=:memory:");
```

## 🤝 Contributing

When adding new features:
1. Write tests first (TDD approach)
2. Ensure all tests pass
3. Maintain or improve coverage
4. Follow existing patterns
5. Update this README for new test categories

## 📊 Current Test Statistics

- **Total Tests**: 210 tests (187 Facts, 23 Theories, all passing)
- **Test Categories**: Unit (Services & Models), Integration, Component
- **Model Tests**: Complete coverage (7/7 models)
- **Service Tests**: Complete coverage (8/9 services)  
- **Component Tests**: Basic coverage (3/30+ pages)
- **Coverage Current**: 7.56% line, 2.98% branch, 8.72% method
- **Coverage Target**: 30% minimum (currently disabled), 70% goal
- **Execution Time**: ~1 second for full suite
- **Database**: In-memory for fast execution

## 🎯 Testing Strategy

### Priority 1: Critical Business Logic
- Invoice creation and payment processing
- Contractor management
- Company and property tracking

### Priority 2: User Interface
- Key page components
- Form validation
- Navigation flows

### Priority 3: Edge Cases
- Error handling
- Boundary conditions
- Concurrent operations

## 📝 Notes

- Tests use in-memory database for speed and isolation
- Each test gets a fresh database instance
- Test data is generated using Bogus faker library
- Component tests use bUnit for Blazor testing
- Coverage excludes auto-generated and migration code

---

**Remember**: A test is not just about code coverage, it's about confidence in your application's behavior.
