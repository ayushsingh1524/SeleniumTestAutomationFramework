using OpenQA.Selenium.Support.UI;
using SauceTesting.Pages;

namespace SauceTesting.Tests;

[TestFixture, Order(6)]
public class FooterTests : BaseTest
{
    private LoginPage loginPage;
    private InventoryPage inventoryPage;

    [SetUp]
    public void PageSetup()
    {
        loginPage = new LoginPage(_driver);
        inventoryPage = new InventoryPage(_driver);
        loginPage.Login(StandardUser, DefaultPassword);
    }

    [Test]
    public void Footer_TwitterLink_ShouldOpenNewTab()
    {
        VerifySocialLink(inventoryPage.ClickTwitter, "twitter.com");
    }

    [Test]
    public void Footer_FacebookLink_ShouldOpenNewTab()
    {
        VerifySocialLink(inventoryPage.ClickFacebook, "facebook.com");
    }

    [Test]
    public void Footer_LinkedInLink_ShouldOpenNewTab()
    {
        VerifySocialLink(inventoryPage.ClickLinkedIn, "linkedin.com");
    }

    private void VerifySocialLink(Action clickAction, string expectedUrlPart)
    {
        string originalWindow = _driver.CurrentWindowHandle;
        
        Assert.That(_driver.WindowHandles.Count, Is.EqualTo(1));

        clickAction();

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        
        wait.Until(d => d.WindowHandles.Count == 2);

        string newWindowHandle = _driver.WindowHandles.First(handle => handle != originalWindow);
        _driver.SwitchTo().Window(newWindowHandle);

        string currentUrl = _driver.Url.ToLower();
        Assert.That(currentUrl, Does.Contain(expectedUrlPart), $"URL did not contain {expectedUrlPart}");

        _driver.Close();
        _driver.SwitchTo().Window(originalWindow);
    }
}