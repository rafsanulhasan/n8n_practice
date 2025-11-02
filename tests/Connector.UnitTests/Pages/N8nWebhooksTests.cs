using Bunit;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using N8N.Connector.Services;
using N8N.AgenticChat.Pages;

using NSubstitute;

using Shouldly;

using BunitTestContext = Bunit.TestContext;

namespace N8N.Connector.UnitTests.Pages;

[TestFixture]
public class N8nWebhooksTests
{
    private BunitTestContext _testContext = null!;
    private N8NWebhookService _mockWebhookService = null!;

    [SetUp]
    public void Setup()
    {
        _testContext = new BunitTestContext();

        // Create a mock service with substituted dependencies
        var mockHttpClient = new HttpClient();
        var mockLogger = Substitute.For<ILogger<N8NWebhookService>>();
        _mockWebhookService = Substitute.ForPartsOf<N8NWebhookService>(mockHttpClient, mockLogger);

        _testContext.Services.AddSingleton(_mockWebhookService);
    }

    [TearDown]
    public void TearDown()
    {
        _testContext?.Dispose();
    }

    [Test]
    public void N8nWebhooks_ShouldRenderCorrectly_OnInitialLoad()
    {
        // Act
        var cut = _testContext.RenderComponent<N8NWebhooks>();

        // Assert - just check key elements are present
        cut.Find("h1").TextContent.ShouldBe("n8n Webhook Triggers");
        cut.Markup.ShouldContain("This page demonstrates how to trigger n8n workflows using webhooks from a Blazor frontend.");
        cut.FindAll(".card").Count.ShouldBe(3);
    }

    [Test]
    public void N8nWebhooks_ShouldDisplayThreeWebhookCards()
    {
        // Act
        var cut = _testContext.RenderComponent<N8NWebhooks>();

        // Assert
        var cards = cut.FindAll(".card");
        cards.Count.ShouldBe(3);
    }

    [Test]
    public void N8nWebhooks_ShouldHaveDefaultFormValues()
    {
        // Act
        var cut = _testContext.RenderComponent<N8NWebhooks>();

        // Assert - Check that default values are bound
        var inputs = cut.FindAll("input");
        inputs[1].GetAttribute("value").ShouldBe("Hello from Blazor!");
        inputs[3].GetAttribute("value").ShouldBe("test@example.com");
        inputs[4].GetAttribute("value").ShouldBe("testuser");
        inputs[5].GetAttribute("value").ShouldBe("password123");
    }

    [Test]
    public void N8nWebhooks_ShouldHaveThreeTriggerButtons()
    {
        // Act
        var cut = _testContext.RenderComponent<N8NWebhooks>();

        // Assert
        var buttons = cut.FindAll("button.btn-primary");
        buttons.Count.ShouldBe(3);
        buttons[0].TextContent.Trim().ShouldContain("Trigger Webhook");
        buttons[1].TextContent.Trim().ShouldContain("Register User");
        buttons[2].TextContent.Trim().ShouldContain("Process Data");
    }

    [Test]
    public void N8nWebhooks_ShouldHaveCorrectCardTitles()
    {
        // Act
        var cut = _testContext.RenderComponent<N8NWebhooks>();

        // Assert
        var cardHeaders = cut.FindAll(".card-header h5");
        cardHeaders.Count.ShouldBe(3);
        cardHeaders[0].TextContent.ShouldBe("Simple Webhook");
        cardHeaders[1].TextContent.ShouldBe("User Registration");
        cardHeaders[2].TextContent.ShouldBe("Data Processing");
    }

    // Note: More complex interaction tests with webhook service mocking would require
    // the service to be interface-based or have virtual methods for NSubstitute to mock properly
}
