using AventStack.ExtentReports;
using Microsoft.Extensions.Configuration;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Safari;

namespace SauceTesting.Tests;

public class BaseTest
{
    protected IWebDriver _driver = null!;

    protected ExtentTest _test;
    protected static IConfiguration _config => Tests.GlobalSetup.Config;

    protected static string DefaultPassword => _config["SauceSettings:Password"]!;
    protected static string StandardUser => _config["SauceSettings:Users:Standard"]!;
    protected static string LockedOutUser => _config["SauceSettings:Users:LockedOut"]!;
    protected static string ProblemUser => _config["SauceSettings:Users:Problem"]!;
    protected static string PerformanceUser => _config["SauceSettings:Users:Performance"]!;
    protected static string ErrorUser => _config["SauceSettings:Users:Error"]!;

    [SetUp]
    public void Setup()
    {
        _test = GlobalSetup.Extent.CreateTest(TestContext.CurrentContext.Test.Name);
        string browserType = _config["BrowserSettings:Type"] ?? "Chrome";
        string baseUrl = _config["BaseUrl"]!;
        bool headless = _config.GetValue<bool>("Headless");

        if (browserType.Equals("Safari", StringComparison.OrdinalIgnoreCase))
        {
            var options = new SafariOptions();

            _driver = new SafariDriver(options);
            _driver.Manage().Window.Maximize();
        }
        else 
        {


            var options = new ChromeOptions();

            bool useCustomBinary = _config.GetValue<bool>("BrowserSettings:UseCustomBinary");
            string binaryPath = _config["BrowserSettings:BinaryPath"]!;
            if (useCustomBinary && !string.IsNullOrEmpty(binaryPath))
            {
                options.BinaryLocation = binaryPath;
            }
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddExcludedArgument("enable-automation");
            options.AddArgument("--window-size=1920,1080");
            if (headless)
            {
                options.AddArgument("--headless=new");
            }
            _driver = new ChromeDriver(options);

        }
        if(_driver == null)
        {
            throw new Exception("WebDriver initialization failed.");
        }
        if (!headless) _driver.Manage().Window.Maximize();
        _driver.Navigate().GoToUrl(baseUrl);
    }

    [TearDown]
    public void Teardown()
    {
        var status = TestContext.CurrentContext.Result.Outcome.Status;
        var message = TestContext.CurrentContext.Result.Message;
        var stacktrace = TestContext.CurrentContext.Result.StackTrace;

        if (status == TestStatus.Failed)
        {
            _test.Fail("Test Failed");
            _test.Fail(message);
            _test.Fail(stacktrace);

            if (_driver != null)
            {
                string base64Img = CaptureScreenshotBase64(_driver);
                _test.Fail("Screenshot of Failure:",
                    MediaEntityBuilder.CreateScreenCaptureFromBase64String(base64Img).Build());
            }
        }
        else if (status == TestStatus.Passed)
        {
            _test.Pass("Test Passed Successfully");
        }
        else
        {
            _test.Skip("Test Skipped");
        }

        if (_driver != null)
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }

    private string CaptureScreenshotBase64(IWebDriver driver)
    {
        ITakesScreenshot ts = (ITakesScreenshot)driver;
        Screenshot screenshot = ts.GetScreenshot();
        return screenshot.AsBase64EncodedString;
    }
}
