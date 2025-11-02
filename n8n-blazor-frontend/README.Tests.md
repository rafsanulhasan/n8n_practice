# N8n Webhook Client - Test Projects

This solution includes comprehensive testing infrastructure with unit tests, integration tests, code coverage, and **mutation testing** to ensure high code quality.

## Test Projects

### 1. N8nWebhookClient.UnitTests
Unit tests for testing individual components, services, and business logic in isolation.

**Testing Framework & Libraries:**
- **NUnit** - Testing framework
- **bUnit** - Blazor component testing
- **NSubstitute** - Mocking framework
- **Bogus** - Fake data generation
- **Shouldly** - Assertion library with readable syntax
- **coverlet.collector** - Code coverage collection

### 2. N8nWebhookClient.IntegrationTests
Integration tests for testing the application with real or containerized dependencies.

**Testing Framework & Libraries:**
- **NUnit** - Testing framework
- **Testcontainers** - Docker container management for tests
- **NSubstitute** - Mocking framework
- **Bogus** - Fake data generation
- **Shouldly** - Assertion library

### 3. Mutation Testing
**Stryker.NET** - Mutation testing framework to verify test effectiveness

## Running Tests

### Run All Tests
```powershell
dotnet test
```

### Run Unit Tests Only
```powershell
dotnet test n8n-blazor-frontend/N8nWebhookClient.UnitTests
```

### Run Integration Tests Only
```powershell
dotnet test n8n-blazor-frontend/N8nWebhookClient.IntegrationTests
```

### Run Tests with Code Coverage
```powershell
dotnet test --collect:"XPlat Code Coverage"
```

### Run Mutation Tests
```powershell
# Run all mutation tests
.\run-mutation-tests.ps1

# Or run for specific project
cd n8n-blazor-frontend/N8nWebhookClient.UnitTests
dotnet stryker
```

### Run Tests with Code Coverage in OpenCover Format (for SonarQube)
```powershell
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover
```

## Test Structure

### Unit Test Example
```csharp
[TestFixture]
public class MyComponentTests : TestContext
{
    private Faker _faker;

    [SetUp]
    public void Setup()
    {
        _faker = new Faker();
    }

    [Test]
    public void Component_ShouldRender_WithExpectedContent()
    {
        // Arrange
        var expectedText = _faker.Lorem.Sentence();

        // Act
        var cut = RenderComponent<MyComponent>(parameters => 
            parameters.Add(p => p.Text, expectedText));

        // Assert
        cut.Find("p").TextContent.ShouldBe(expectedText);
    }
}
```

### Integration Test Example with Testcontainers
```csharp
[TestFixture]
public class WebhookIntegrationTests
{
    private IContainer _container;
    
    [OneTimeSetUp]
    public async Task Setup()
    {
        _container = new ContainerBuilder()
            .WithImage("n8nio/n8n:latest")
            .WithPortBinding(5678, true)
            .Build();
        
        await _container.StartAsync();
    }

    [Test]
    public async Task Webhook_ShouldProcessRequest_Successfully()
    {
        // Arrange
        var payload = new { data = "test" };

        // Act
        // Send HTTP request to container

        // Assert
        // Verify response
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _container.DisposeAsync();
    }
}
```

## Assertion Examples with Shouldly

```csharp
// Equality
result.ShouldBe(expected);

// Null checks
result.ShouldNotBeNull();
result.ShouldBeNullOrEmpty();

// Collections
list.ShouldContain(item);
list.ShouldBeEmpty();
list.Count.ShouldBe(5);

// Ranges
age.ShouldBeInRange(18, 65);

// Strings
text.ShouldContain("expected");
text.ShouldStartWith("Hello");
text.ShouldEndWith("world");

// Exceptions
Should.Throw<ArgumentException>(() => method());
```

## Mocking with NSubstitute

```csharp
// Create mock
var mock = Substitute.For<IMyService>();

// Setup return value
mock.GetData().Returns("test data");

// Verify method was called
mock.Received(1).GetData();
mock.DidNotReceive().DeleteData();
```

## Fake Data Generation with Bogus

```csharp
// Simple fake data
var name = new Faker().Name.FullName();
var email = new Faker().Internet.Email();

// Complex object generation
var person = new Faker<Person>()
    .RuleFor(p => p.Name, f => f.Name.FullName())
    .RuleFor(p => p.Email, f => f.Internet.Email())
    .RuleFor(p => p.Age, f => f.Random.Int(18, 65))
    .Generate();

// Generate multiple objects
var people = new Faker<Person>()
    .RuleFor(p => p.Name, f => f.Name.FullName())
    .Generate(10); // Generate 10 people
```

## Code Coverage Reports

Coverage reports are automatically generated when running tests with coverage collection. The reports are stored in the `./coverage` directory.

### View Coverage Report
After running tests with coverage, you can find the coverage report in:
- `./coverage/**/coverage.opencover.xml` - OpenCover format (for SonarQube)
- `./coverage/**/coverage.cobertura.xml` - Cobertura format

## CI/CD Integration

The GitHub Actions workflow automatically:
1. Builds the solution
2. Runs all tests with code coverage
3. Sends coverage results to SonarQube for analysis

## Best Practices

1. **Use descriptive test names** - Test names should describe what is being tested and the expected outcome
2. **Follow AAA pattern** - Arrange, Act, Assert
3. **One assertion per test** - Keep tests focused on a single behavior
4. **Use test fixtures** - Group related tests together
5. **Mock external dependencies** - Unit tests should not depend on external services
6. **Use Testcontainers for integration tests** - Spin up real dependencies in containers
7. **Generate realistic test data** - Use Bogus to create meaningful test data
8. **Keep tests fast** - Unit tests should complete in milliseconds
9. **Make tests independent** - Tests should not depend on each other
10. **Aim for high coverage** - Target >80% code coverage

## Troubleshooting

### Tests not discovering
- Ensure test projects are built: `dotnet build`
- Check that test methods have `[Test]` attribute
- Verify test classes have `[TestFixture]` attribute

### Docker/Testcontainers issues
- Ensure Docker is running
- Check Docker has permission to pull images
- Verify Docker Desktop is configured for your platform

### Coverage not generating
- Ensure coverlet.collector package is installed
- Use correct command with coverage collection flag
- Check that tests are actually running

## Mutation Testing

Mutation testing evaluates the **quality** of your tests by introducing small code changes (mutations) and checking if tests catch them.

### What Gets Measured
- **Mutation Score**: % of mutations killed by tests
- **Test Effectiveness**: How well tests detect bugs
- **Test Quality**: Beyond simple code coverage

### Running Mutation Tests
```powershell
# All projects
.\run-mutation-tests.ps1

# Specific project
cd n8n-blazor-frontend/N8nWebhookClient.UnitTests
dotnet stryker
```

### Understanding Results
- **80-100%**: Excellent test quality ✅
- **60-79%**: Good test quality 👍
- **40-59%**: Acceptable, needs improvement ⚠️
- **<40%**: Poor test quality, critical gaps ❌

### View Reports
After running Stryker, open the HTML report:
```
n8n-blazor-frontend/N8nWebhookClient.UnitTests/StrykerOutput/[timestamp]/reports/mutation-report.html
```

For detailed information, see [MUTATION_TESTING.md](../MUTATION_TESTING.md)

## Resources

- [NUnit Documentation](https://docs.nunit.org/)
- [bUnit Documentation](https://bunit.dev/)
- [NSubstitute Documentation](https://nsubstitute.github.io/)
- [Bogus Documentation](https://github.com/bchavez/Bogus)
- [Shouldly Documentation](https://docs.shouldly.org/)
- [Testcontainers Documentation](https://dotnet.testcontainers.org/)
- [Stryker.NET Documentation](https://stryker-mutator.io/docs/stryker-net/introduction/)
