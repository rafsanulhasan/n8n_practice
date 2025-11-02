using Bunit;

using N8N.AgenticChat.Pages;

using Shouldly;

using BunitTestContext = Bunit.TestContext;

namespace N8N.Connector.UnitTests.Pages;

[TestFixture]
public class HomeTests : BunitTestContext
{
    [Test]
    public void Home_ShouldRender_WithWelcomeMessage()
    {
        // Act
        var cut = RenderComponent<Home>();

        // Assert
        cut.Find("h1").TextContent.ShouldBe("Hello, world!");
    }

    [Test]
    public void Home_ShouldDisplay_WelcomeText()
    {
        // Act
        var cut = RenderComponent<Home>();

        // Assert
        cut.Markup.ShouldContain("Welcome to your new app.");
    }

    [Test]
    public void Home_ShouldHave_PageTitle()
    {
        // Act
        var cut = RenderComponent<Home>();

        // Assert
        var pageTitle = cut.FindComponent<Microsoft.AspNetCore.Components.Web.PageTitle>();
        pageTitle.Instance.ChildContent.ShouldNotBeNull();
    }

    [Test]
    public void Home_ShouldHave_Heading()
    {
        // Act
        var cut = RenderComponent<Home>();

        // Assert
        var heading = cut.Find("h1");
        heading.ShouldNotBeNull();
        heading.TextContent.ShouldNotBeNullOrEmpty();
    }
}
