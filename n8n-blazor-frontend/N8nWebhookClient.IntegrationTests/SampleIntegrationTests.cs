using Bogus;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace N8nWebhookClient.IntegrationTests;

/// <summary>
/// Sample integration test class demonstrating Testcontainers usage with NUnit
/// </summary>
[TestFixture]
public class SampleIntegrationTests
{
    private Faker _faker = null!;
    private IContainer? _testContainer;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        // Initialize Faker for generating test data
        _faker = new Faker();

        // Example: Create a generic container for testing
        // Uncomment and customize based on your needs
        /*
        _testContainer = new ContainerBuilder()
            .WithImage("n8nio/n8n:latest")
            .WithPortBinding(5678, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(5678)))
            .Build();
        
        await _testContainer.StartAsync();
        */
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        // Clean up containers after all tests
        if (_testContainer != null)
        {
            await _testContainer.DisposeAsync();
        }
    }

    [SetUp]
    public void Setup()
    {
        // Setup before each test
    }

    [Test]
    public async Task IntegrationTest_ShouldConnectToService_Successfully()
    {
        // Arrange
        var testData = _faker.Lorem.Paragraph();

        // Act
        // Simulate an integration test
        await Task.Delay(10); // Simulating async operation
        var result = testData;

        // Assert
        result.ShouldNotBeNullOrEmpty();
        result.ShouldBe(testData);
    }

    [Test]
    public async Task WebhookEndpoint_ShouldReceiveData_WhenTriggered()
    {
        // Arrange
        var webhookPayload = new WebhookPayload
        {
            EventType = _faker.Lorem.Word(),
            Data = _faker.Lorem.Sentence(),
            Timestamp = DateTime.UtcNow
        };

        // Act
        // In a real scenario, you would send HTTP request to webhook endpoint
        await Task.Delay(10);

        // Assert
        webhookPayload.EventType.ShouldNotBeNullOrEmpty();
        webhookPayload.Data.ShouldNotBeNullOrEmpty();
        webhookPayload.Timestamp.ShouldBeLessThanOrEqualTo(DateTime.UtcNow);
    }

    [Test]
    [TestCase("event1")]
    [TestCase("event2")]
    [TestCase("event3")]
    public async Task ParameterizedIntegrationTest_ShouldProcessEvents_ForDifferentTypes(string eventType)
    {
        // Arrange
        var payload = new { EventType = eventType, Data = _faker.Lorem.Sentence() };

        // Act
        await Task.Delay(10);

        // Assert
        payload.EventType.ShouldBe(eventType);
        payload.Data.ShouldNotBeNullOrEmpty();
    }

    [Test]
    public void MockedDependency_ShouldWork_InIntegrationTests()
    {
        // Arrange
        var mockLogger = Substitute.For<ITestLogger>();
        var message = _faker.Lorem.Sentence();

        // Act
        mockLogger.Log(message);

        // Assert
        mockLogger.Received(1).Log(message);
    }
}

// Sample models for testing
public class WebhookPayload
{
    public string EventType { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public interface ITestLogger
{
    void Log(string message);
}
