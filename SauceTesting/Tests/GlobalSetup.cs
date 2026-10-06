using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.IO;

namespace SauceTesting.Tests;

[SetUpFixture]
public class GlobalSetup
{
    public static ExtentReports Extent;
    public static IConfiguration Config;

    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        string workDir = TestContext.CurrentContext.TestDirectory;
        var builder = new ConfigurationBuilder()
            .SetBasePath(workDir)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        Config = builder.Build();

        string reportPath = Path.Combine(workDir, "Reports");
        if (!Directory.Exists(reportPath))
        {
            Directory.CreateDirectory(reportPath);
        }

        var htmlReporter = new ExtentSparkReporter(Path.Combine(reportPath, "index.html"));
        htmlReporter.Config.DocumentTitle = "SauceDemo Test Report";
        htmlReporter.Config.ReportName = "Automation Status";

        Extent = new ExtentReports();
        Extent.AttachReporter(htmlReporter);
    }

    [OneTimeTearDown]
    public void RunAfterAllTests()
    {
        Extent.Flush();
    }
}