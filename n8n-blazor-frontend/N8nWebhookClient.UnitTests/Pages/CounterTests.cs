using Bunit;
using N8nWebhookClient.Pages;
using NUnit.Framework;
using Shouldly;
using BunitTestContext = Bunit.TestContext;

namespace N8nWebhookClient.UnitTests.Pages;

[TestFixture]
public class CounterTests : BunitTestContext
{
    [Test]
    public void Counter_ShouldRender_WithInitialCount()
    {
        // Act
        var cut = RenderComponent<Counter>();

        // Assert
        cut.Find("h1").TextContent.ShouldBe("Counter");
        cut.Find("p[role='status']").TextContent.ShouldBe("Current count: 0");
    }

    [Test]
    public void Counter_ShouldHave_IncrementButton()
    {
        // Act
        var cut = RenderComponent<Counter>();

        // Assert
        var button = cut.Find("button.btn.btn-primary");
        button.ShouldNotBeNull();
        button.TextContent.ShouldBe("Click me");
    }

    [Test]
    public void Counter_ShouldIncrement_WhenButtonClicked()
    {
        // Arrange
        var cut = RenderComponent<Counter>();
        var button = cut.Find("button");

        // Act
        button.Click();

        // Assert
        cut.Find("p[role='status']").TextContent.ShouldBe("Current count: 1");
    }

    [Test]
    public void Counter_ShouldIncrementMultipleTimes_WhenButtonClickedMultipleTimes()
    {
        // Arrange
        var cut = RenderComponent<Counter>();
        var button = cut.Find("button");

        // Act
        button.Click();
        button.Click();
        button.Click();

        // Assert
        cut.Find("p[role='status']").TextContent.ShouldBe("Current count: 3");
    }

    [Test]
    public void Counter_ShouldMaintainState_BetweenClicks()
    {
        // Arrange
        var cut = RenderComponent<Counter>();
        var button = cut.Find("button");

        // Act & Assert
        cut.Find("p[role='status']").TextContent.ShouldBe("Current count: 0");

        button.Click();
        cut.Find("p[role='status']").TextContent.ShouldBe("Current count: 1");

        button.Click();
        cut.Find("p[role='status']").TextContent.ShouldBe("Current count: 2");

        button.Click();
        cut.Find("p[role='status']").TextContent.ShouldBe("Current count: 3");
    }

    [Test]
    public void Counter_ShouldHave_PageTitle()
    {
        // Act
        var cut = RenderComponent<Counter>();

        // Assert
        var pageTitle = cut.FindComponent<Microsoft.AspNetCore.Components.Web.PageTitle>();
        pageTitle.Instance.ChildContent.ShouldNotBeNull();
    }
}
