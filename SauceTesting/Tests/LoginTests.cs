using SauceTesting.Pages;
using System.Diagnostics;

namespace SauceTesting.Tests;

[TestFixture, Order(1)]
public class LoginTest : BaseTest
{
    private LoginPage loginPage;
    private InventoryPage inventoryPage;

    [SetUp]
    public void PageSetup()
    {
        loginPage = new LoginPage(_driver);
        inventoryPage = new InventoryPage(_driver);
    }

    [TestCase("standard_user")]
    [TestCase("problem_user")]
    [TestCase("performance_glitch_user")]
    [TestCase("error_user")]
    [TestCase("visual_user")]
    public void ValidLogin_SeveralUsers_ShouldLoginSuccessfully(string username)
    {
        loginPage.Login(username, DefaultPassword);
        Assert.That(inventoryPage.GetPageTitle(), Is.EqualTo("Products"));
    }


    [Test]
    public void InvalidLogin_LockedOutUser_ShouldShowError_And_VisualCues()
    {
        loginPage.Login(LockedOutUser, DefaultPassword);
        string error = loginPage.GetErrorMessage();
        Assert.That(error, Does.Contain("Sorry, this user has been locked out"), "Error message text is incorrect.");

        Assert.That(loginPage.AreErrorIconsVisible(), Is.True,
            "Visual Error Icons (Red X) did not appear on the input fields.");

        Assert.That(loginPage.AreInputFieldsRed(), Is.True,
            "Input fields did not turn red (missing 'input_error' class).");
    }

    [Test]
    public void InvalidPassword_ShouldSeeErrorMessage()
    {
        loginPage.Login(StandardUser, "wrong_password");

        Assert.That(loginPage.GetErrorMessage(),
            Does.Contain("Username and password do not match"));
    }

    [Test]
    public void PerformanceGlitchUser_Login_ShouldTakeLongerThanExpected()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        loginPage.Login(PerformanceUser, DefaultPassword);

        stopwatch.Stop();

        Assert.That(stopwatch.Elapsed.TotalSeconds, Is.GreaterThan(3.0));

    }

    [Test]
    public void Logout_Successful_ShouldRedirectToLogin()
    {
        loginPage.Login(StandardUser, DefaultPassword);

        inventoryPage.Logout();

        Assert.That(loginPage.IsLoginButtonVisible(), Is.True, "Login button should be visible after logout.");

        Assert.That(_driver.Url, Is.Not.EqualTo("https://www.saucedemo.com/inventory.html"),
            "User should not remain on inventory page.");
    }
}
