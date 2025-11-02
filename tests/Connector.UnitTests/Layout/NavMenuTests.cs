using Bunit;

using N8N.AgenticChat.Layout;

using Shouldly;

using BunitTestContext = Bunit.TestContext;

namespace N8N.Connector.UnitTests.Layout;

[TestFixture]
public class NavMenuTests
{
    private BunitTestContext _testContext = null!;

    [SetUp]
    public void Setup()
    {
        _testContext = new BunitTestContext();
    }

    [TearDown]
    public void TearDown()
    {
        _testContext?.Dispose();
    }

    [Test]
    public void NavMenu_ShouldRenderCorrectly_OnInitialLoad()
    {
        // Act
        var cut = _testContext.RenderComponent<NavMenu>();

        // Assert
        cut.Find(".navbar-brand").TextContent.ShouldBe("N8nWebhookClient");
    }

    [Test]
    public void NavMenu_ShouldBeCollapsed_Initially()
    {
        // Act
        var cut = _testContext.RenderComponent<NavMenu>();

        // Assert
        var navMenu = cut.Find(".nav-scrollable");
        navMenu.ClassList.ShouldContain("collapse");
    }

    [Test]
    public void NavMenu_ShouldExpand_WhenTogglerClicked()
    {
        // Arrange
        var cut = _testContext.RenderComponent<NavMenu>();

        // Act
        var toggler = cut.Find(".navbar-toggler");
        toggler.Click();

        // Assert
        var navMenu = cut.Find(".nav-scrollable");
        navMenu.ClassList.ShouldNotContain("collapse");
    }

    [Test]
    public void NavMenu_ShouldCollapse_WhenTogglerClickedTwice()
    {
        // Arrange
        var cut = _testContext.RenderComponent<NavMenu>();
        var toggler = cut.Find(".navbar-toggler");

        // Act
        toggler.Click(); // Expand
        toggler.Click(); // Collapse

        // Assert
        var navMenu = cut.Find(".nav-scrollable");
        navMenu.ClassList.ShouldContain("collapse");
    }

    [Test]
    public void NavMenu_ShouldCollapse_WhenNavMenuClicked()
    {
        // Arrange
        var cut = _testContext.RenderComponent<NavMenu>();
        var toggler = cut.Find(".navbar-toggler");
        toggler.Click(); // Expand first

        // Act
        var navMenu = cut.Find(".nav-scrollable");
        navMenu.Click();

        // Assert
        navMenu.ClassList.ShouldContain("collapse");
    }

    [Test]
    public void NavMenu_ShouldHaveFourNavigationLinks()
    {
        // Act
        var cut = _testContext.RenderComponent<NavMenu>();

        // Assert
        var links = cut.FindAll(".nav-link");
        links.Count.ShouldBe(4);
    }

    [Test]
    public void NavMenu_ShouldHaveHomeLink()
    {
        // Act
        var cut = _testContext.RenderComponent<NavMenu>();

        // Assert
        var homeLink = cut.Find(".nav-link[href='']");
        homeLink.TextContent.Trim().ShouldContain("Home");
    }

    [Test]
    public void NavMenu_ShouldHaveCounterLink()
    {
        // Act
        var cut = _testContext.RenderComponent<NavMenu>();

        // Assert
        var counterLink = cut.Find(".nav-link[href='counter']");
        counterLink.TextContent.Trim().ShouldContain("Counter");
    }

    [Test]
    public void NavMenu_ShouldHaveWeatherLink()
    {
        // Act
        var cut = _testContext.RenderComponent<NavMenu>();

        // Assert
        var weatherLink = cut.Find(".nav-link[href='weather']");
        weatherLink.TextContent.Trim().ShouldContain("Weather");
    }

    [Test]
    public void NavMenu_ShouldHaveN8nWebhooksLink()
    {
        // Act
        var cut = _testContext.RenderComponent<NavMenu>();

        // Assert
        var webhooksLink = cut.Find(".nav-link[href='n8n-webhooks']");
        webhooksLink.TextContent.Trim().ShouldContain("n8n Webhooks");
    }

    [Test]
    public void NavMenu_ShouldHaveTogglerButton()
    {
        // Act
        var cut = _testContext.RenderComponent<NavMenu>();

        // Assert
        var toggler = cut.Find(".navbar-toggler");
        toggler.ShouldNotBeNull();
        toggler.GetAttribute("title").ShouldBe("Navigation menu");
    }

    [Test]
    public void NavMenu_ShouldHaveTogglerIcon()
    {
        // Act
        var cut = _testContext.RenderComponent<NavMenu>();

        // Assert
        var togglerIcon = cut.Find(".navbar-toggler-icon");
        togglerIcon.ShouldNotBeNull();
    }

    [Test]
    public void NavMenu_ShouldHaveCorrectNavBarClasses()
    {
        // Act
        var cut = _testContext.RenderComponent<NavMenu>();

        // Assert
        var topRow = cut.Find(".top-row");
        topRow.ClassList.ShouldContain("navbar");
        topRow.ClassList.ShouldContain("navbar-dark");
    }

    [Test]
    public void NavMenu_ShouldToggleMultipleTimes_Correctly()
    {
        // Arrange
        var cut = _testContext.RenderComponent<NavMenu>();
        var toggler = cut.Find(".navbar-toggler");
        var navMenu = cut.Find(".nav-scrollable");

        // Act & Assert - Click 3 times
        toggler.Click(); // Expand
        navMenu.ClassList.ShouldNotContain("collapse");

        toggler.Click(); // Collapse
        navMenu.ClassList.ShouldContain("collapse");

        toggler.Click(); // Expand again
        navMenu.ClassList.ShouldNotContain("collapse");
    }
}
