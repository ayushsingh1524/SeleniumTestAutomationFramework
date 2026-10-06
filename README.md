# SauceDemo E2E Automation Framework

## Project Overview
This project is a comprehensive **Quality Assurance & Testing** solution for an E-commerce Web App ([Swag Labs](https://www.saucedemo.com/)).

**Key Achievements:**
* Automated 18 test scripts with **Selenium WebDriver** in **C#** using Page Object Model, explicit waits, and data-driven **NUnit** tests covering login, cart, checkout, and invalid-input flows.
* Built a parallel 08-test Selenium suite in **JavaScript** using **Mocha**.
* Executed both automation suites through **GitHub Actions** on every push.
* Logged 05 defects in **Jira** with reproduction steps, severity, and expected vs. actual results; wrote a test plan and 30 test cases for regression coverage.

The C# framework implements the **Page Object Model (POM)** design pattern to ensure maintainability, readability, and scalability, including HTML reporting, configuration management, and automatic screenshot capture on failure.
## Tech Stack
*   **Language:** C# (.NET 8.0)
*   **Automation Tool:** Selenium WebDriver
*   **Browser:** Google Chrome (configured via options)
*   **Test Framework:** NUnit
*   **Reporting:** ExtentReports (HTML Reports)
*   **Configuration:** Microsoft.Extensions.Configuration (JSON)
*   **Waits:** Selenium Extras `ExpectedConditions` (Explicit Waits)

## Design Pattern: Page Object Model (POM)
The project is structured to separate the **Test Logic** from the **UI Elements/Actions**.

*   **`Pages/` Directory:** Contains classes representing web pages (`LoginPage`, `InventoryPage`, `CartPage`, `Checkout*Page`). These classes handle element locators and user interactions.
*   **`Tests/` Directory:** Contains the actual test assertions. These classes do not interact with the browser directly; they call methods from the Page classes.
*   **`BaseTest` & `BasePage`:** Handles common setup (Browser initialization, Reporting, Teardown) and generic actions (Click, Type, Wait).

## Key Features
*   **Data-Driven Testing:** Uses NUnit `[TestCase]` to validate multiple login scenarios (Standard, Problem, Error, Visual users) in a single run.
*   **Robust Configuration:** Uses `appsettings.json` to manage credentials, URLs, and browser settings without hardcoding.
*   **HTML Reporting:** Generates detailed execution reports in the `Reports/` folder.
*   **Failure Analysis:** Automatically captures and embeds **screenshots** into the report when a test fails.

## Test Scenarios & Coverage

The framework covers scenarios for various user types provided by SauceDemo:

### 1. Standard User (`standard_user`)
*   **Type:** Happy Path Testing.
*   **Scenarios:**
    *   [x] Successful login.
    *   [x] Inventory sorting (Price, Name).
    *   [x] Adding/Removing items from the cart.
    *   [x] Cart persistence validation.
    *   [x] Full checkout process (Information input -> Tax calculation verification -> Completion).
    *   [x] Social Media footer link verification.
    *   [x] Logout functionality.

### 2. Locked Out User (`locked_out_user`)
*   **Type:** Negative Testing.
*   **Scenarios:**
    *   [x] Verifies that the system prevents login.
    *   [x] Asserts the correct error message: *"Sorry, this user has been locked out."*
    *   [x] Validates visual cues (Red input fields and error icons).

### 3. Problem User (`problem_user`)
*   **Type:** Functional/UI Bug Testing.
*   **Scenarios:**
    *   [x] **Broken Image Verification:** Detected by checking image source attributes in the Inventory.

### 4. Performance Glitch User (`performance_glitch_user`)
*   **Type:** Non-Functional / Performance Testing.
*   **Scenarios:**
    *   [x] **Login Latency:** Measures login time using `Stopwatch`.
    *   [x] Asserts that login takes longer than the standard threshold to verify the system behavior.

### 5. Error User (`error_user`)
*   **Type:** JavaScript/Logic Error Testing.
*   **Scenarios:**
    *   [x] **Checkout Failure:** Verifies that this user **cannot** complete a purchase. The test specifically checks that clicking "Finish" fails to redirect to the confirmation page (due to a known site bug for this user).

---

## Configuration (`appsettings.json`)

This project uses an `appsettings.json` file to manage environment variables. Ensure this file exists in the root directory and is set to **"Copy to Output Directory: Always"**.

```json
{
  "BrowserSettings": {
    "UseCustomBinary": true,
    "BinaryPath": "/Path/To/Your/Chrome/Or/Brave/Binary"
  },
  "Headless": false,
  "BaseUrl": "https://www.saucedemo.com/",
  "SauceSettings": {
    "Password": "secret_sauce",
    "Users": {
      "Standard": "standard_user",
      "LockedOut": "locked_out_user",
      "Problem": "problem_user",
      "Performance": "performance_glitch_user",
      "Error": "error_user",
      "Visual": "visual_user"
    }
  }
}
```

## Project Structure

```text
SauceTesting/
├── Pages/                      # Page Object Classes
│   ├── BasePage.cs             # Common WebDriver methods
│   ├── LoginPage.cs            # Auth interactions
│   ├── InventoryPage.cs        # Catalog & Sorting
│   ├── ProductDetailsPage.cs   # Individual Item View
│   ├── CartPage.cs             # Cart Management
│   ├── CheckoutStepOnePage.cs  # User Info Input
│   ├── CheckoutStepTwoPage.cs  # Overview & Totals
│   └── CheckoutCompletePage.cs # Order Confirmation
│   
├── Tests/                      # NUnit Test Classes
│   ├── BaseTest.cs             # Setup, Teardown, Reporting, Screenshots
│   ├── LoginTests.cs           # Auth & Performance Tests
│   ├── InventoryTests.cs       # Sorting & Broken Images
│   ├── ProductDetailsTest.cs   # Product View Logic
│   ├── CartTests.cs            # Add/Remove logic
│   ├── CheckoutTests.cs        # E2E Purchase Flow
│   └── FooterTests.cs          # External Link Verification
│
├── Enums/
│   └── SortOptions.cs          # Sorting constants
│   
├── appsettings.json            # Configuration file
└── SauceTesting.sln            # Solution file
```

## Test Reports

After executing the tests, an HTML report is generated automatically using **ExtentReports**.

*   **Location:** `SauceTesting/bin/Debug/net8.0/Reports/index.html`
*   **Content:**
    *   Pass/Fail status for every test.
    *   Execution duration.
    *   **Screenshots:** If a test fails, a base64 screenshot of the browser state is embedded directly into the report.

## How to Run

### Prerequisites
1.  **.NET SDK** installed.
2.  **Visual Studio** or **VS Code** (with C# Dev Kit).
3.  **Google Chrome** installed.

### Execution
**Option 1: Command Line**
```bash
dotnet test
```

**Option 2: Visual Studio / VS Code**
1.  Open the Solution.
2.  Open **Test Explorer**.
3.  Click **Run All Tests**.