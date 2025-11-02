using Bogus;
using BunitTestContext = Bunit.TestContext;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;

namespace N8nWebhookClient.UnitTests;

/// <summary>
/// Sample unit test class demonstrating best practices with NUnit, bUnit, NSubstitute, Bogus, and Shouldly
/// </summary>
[TestFixture]
public class SampleComponentTests : BunitTestContext
{
    private Faker _faker = null!;

    [SetUp]
    public void Setup()
    {
        // Initialize Faker for generating test data
        _faker = new Faker();

        // Setup test context for Blazor components
        Services.AddSingleton<ITestService>(Substitute.For<ITestService>());
    }
    [TearDown]
    public void TearDown()
    {
        // Clean up after each test
        Dispose();
    }

    [Test]
    public void SampleTest_ShouldPass_WhenConditionsAreMet()
    {
        // Arrange
        var expectedValue = _faker.Random.Int(1, 100);

        // Act
        var actualValue = expectedValue;

        // Assert using Shouldly
        actualValue.ShouldBe(expectedValue);
    }

    [Test]
    public void MockedService_ShouldReturnExpectedData_WhenCalled()
    {
        // Arrange
        var mockService = Substitute.For<ITestService>();
        var expectedData = _faker.Lorem.Sentence();
        mockService.GetData().Returns(expectedData);

        // Act
        var result = mockService.GetData();

        // Assert
        result.ShouldBe(expectedData);
        mockService.Received(1).GetData();
    }

    [Test]
    [TestCase(1, 2, 3)]
    [TestCase(5, 5, 10)]
    [TestCase(-1, 1, 0)]
    public void ParameterizedTest_ShouldCalculateSum_ForMultipleInputs(int a, int b, int expected)
    {
        // Act
        var result = a + b;

        // Assert
        result.ShouldBe(expected);
    }

    [Test]
    public void BogusDataGeneration_ShouldCreateValidTestData()
    {
        // Arrange & Act
        var testPerson = new Faker<TestPerson>()
            .RuleFor(p => p.Name, f => f.Name.FullName())
            .RuleFor(p => p.Email, f => f.Internet.Email())
            .RuleFor(p => p.Age, f => f.Random.Int(18, 65))
            .Generate();

        // Assert
        testPerson.ShouldNotBeNull();
        testPerson.Name.ShouldNotBeNullOrEmpty();
        testPerson.Email.ShouldContain("@");
        testPerson.Age.ShouldBeInRange(18, 65);
    }
}

// Sample interfaces and classes for testing
public interface ITestService
{
    string GetData();
}

public class TestPerson
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Age { get; set; }
}
