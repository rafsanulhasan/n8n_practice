using Bunit;
using N8nWebhookClient.Layout;
using NUnit.Framework;
using Shouldly;
using BunitTestContext = Bunit.TestContext;

namespace N8nWebhookClient.UnitTests.Layout;

[TestFixture]
public class MainLayoutTests
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
    public void MainLayout_ShouldRenderCorrectly()
    {
        // Act
        var cut = _testContext.RenderComponent<MainLayout>();

        // Assert
        var page = cut.Find(".page");
        page.ShouldNotBeNull();
    }

    [Test]
    public void MainLayout_ShouldHaveSidebar()
    {
        // Act
        var cut = _testContext.RenderComponent<MainLayout>();

        // Assert
        var sidebar = cut.Find(".sidebar");
        sidebar.ShouldNotBeNull();
    }

    [Test]
    public void MainLayout_ShouldContainNavMenu()
    {
        // Act
        var cut = _testContext.RenderComponent<MainLayout>();

        // Assert
        var navMenu = cut.FindComponent<NavMenu>();
        navMenu.ShouldNotBeNull();
    }

    [Test]
    public void MainLayout_ShouldHaveMainElement()
    {
        // Act
        var cut = _testContext.RenderComponent<MainLayout>();

        // Assert
        var main = cut.Find("main");
        main.ShouldNotBeNull();
    }

    [Test]
    public void MainLayout_ShouldHaveTopRow()
    {
        // Act
        var cut = _testContext.RenderComponent<MainLayout>();

        // Assert
        var topRow = cut.Find(".top-row");
        topRow.ShouldNotBeNull();
        topRow.ClassList.ShouldContain("ps-3"); // ps-3 is used, not px-4
    }

    [Test]
    public void MainLayout_ShouldHaveAboutLink()
    {
        // Act
        var cut = _testContext.RenderComponent<MainLayout>();

        // Assert
        var aboutLink = cut.Find("a[href='https://learn.microsoft.com/aspnet/core/']");
        aboutLink.ShouldNotBeNull();
        aboutLink.TextContent.ShouldBe("About");
        aboutLink.GetAttribute("target").ShouldBe("_blank");
    }

    [Test]
    public void MainLayout_ShouldHaveArticleElement()
    {
        // Act
        var cut = _testContext.RenderComponent<MainLayout>();

        // Assert
        var article = cut.Find("article");
        article.ShouldNotBeNull();
        article.ClassList.ShouldContain("content");
        article.ClassList.ShouldContain("px-4");
    }

    [Test]
    public void MainLayout_ShouldHaveCorrectStructure()
    {
        // Act
        var cut = _testContext.RenderComponent<MainLayout>();

        // Assert
        var page = cut.Find(".page");
        var sidebar = page.QuerySelector(".sidebar");
        var main = page.QuerySelector("main");

        sidebar.ShouldNotBeNull();
        main.ShouldNotBeNull();
    }

    [Test]
    public void MainLayout_ShouldRenderBodyContent()
    {
        // Act
        var cut = _testContext.RenderComponent<MainLayout>(parameters => parameters
            .Add(p => p.Body, "<div id='test-content'>Test Body Content</div>"));

        // Assert
        var content = cut.Find("#test-content");
        content.ShouldNotBeNull();
        content.TextContent.ShouldBe("Test Body Content");
    }
}
