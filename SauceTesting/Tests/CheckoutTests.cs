using SauceTesting.Pages;

namespace SauceTesting.Tests;

[TestFixture, Order(4)]
public class CheckoutTests : BaseTest
{
    private LoginPage loginPage;
    private InventoryPage inventoryPage;
    private CartPage cartPage;
    private CheckoutStepOnePage checkoutStepOne;
    private CheckoutStepTwoPage checkoutStepTwo;
    private CheckoutCompletePage checkoutComplete;

    [SetUp]
    public void PageSetup()
    {
        loginPage = new LoginPage(_driver);
        inventoryPage = new InventoryPage(_driver);
        cartPage = new CartPage(_driver);
        checkoutStepOne = new CheckoutStepOnePage(_driver);
        checkoutStepTwo = new CheckoutStepTwoPage(_driver);
        checkoutComplete = new CheckoutCompletePage(_driver);

        loginPage.Login(StandardUser, DefaultPassword);
    }

    private void NavigateToCheckout()
    {
        inventoryPage.AddToCart("Sauce Labs Backpack");
        inventoryPage.GoToCart();
        cartPage.Checkout();
    }

    [TestCase("", "Alkhateeb", "00000", "Error: First Name is required")]
    [TestCase("Mayas", "", "00000", "Error: Last Name is required")]
    [TestCase("Mayas", "Alkhateeb", "", "Error: Postal Code is required")]
    public void Checkout_Validation_MissingFields_ShouldShowCorrectError(
        string firstName, string lastName, string zip, string expectedErrorMessage)
    {
        NavigateToCheckout();

        checkoutStepOne.EnterDetails(firstName, lastName, zip);
        checkoutStepOne.Continue();

        string error = checkoutStepOne.GetErrorMessage();
        Assert.That(error, Is.EqualTo(expectedErrorMessage),
            $"Error message mismatch for missing field scenario: {expectedErrorMessage}");
    }

    [Test]
    public void E2E_SuccessfulPurchase_ShouldCompleteFlow()
    {
        inventoryPage.AddToCart("Sauce Labs Backpack"); // $29.99
        inventoryPage.AddToCart("Sauce Labs Bike Light"); // $9.99

        inventoryPage.GoToCart();
        cartPage.Checkout();

        checkoutStepOne.EnterDetails("Mayas", "kh", "00000");
        checkoutStepOne.Continue();

        decimal sumOfItems = checkoutStepTwo.SumOfVisibleItemPrices();
        decimal subtotal = checkoutStepTwo.GetSubtotal();
        decimal tax = checkoutStepTwo.GetTax();
        decimal total = checkoutStepTwo.GetTotal();

        Assert.That(subtotal, Is.EqualTo(sumOfItems), "Subtotal does not match sum of item prices");

        Assert.That(total, Is.EqualTo(subtotal + tax), "Total calculation is incorrect");


        checkoutStepTwo.Finish();

        Assert.That(checkoutComplete.GetCompleteHeaderText(), Is.EqualTo("Thank you for your order!"));
    }

    [Test]
    public void Navigation_CheckoutStepOne_Cancel_ShouldReturnToCart()
    {
        NavigateToCheckout();

        checkoutStepOne.Cancel();

        Assert.That(_driver.Url, Does.Contain("cart.html"), "User was not redirected to the Cart page.");

        Assert.That(cartPage.GetCartItemNames().Count, Is.GreaterThan(0), "Cart should not be empty after cancelling.");
    }

    [Test]
    public void Navigation_CheckoutStepTwo_Cancel_ShouldReturnToInventory()
    {
        NavigateToCheckout();

        checkoutStepOne.EnterDetails("Mayas", "Kh", "00000");
        checkoutStepOne.Continue();

        checkoutStepTwo.Cancel();

        Assert.That(inventoryPage.GetPageTitle(), Is.EqualTo("Products"), "User was not redirected to the Inventory page.");
    }

    [Test]
    public void ErrorUser_Checkout_FinishButton_ShouldFailToCompleteOrder()
    {

        inventoryPage.Logout();
        loginPage.Login(ErrorUser, DefaultPassword);

        string firstItemName = inventoryPage.GetItemNames().First();

        inventoryPage.AddToCart(firstItemName);
        inventoryPage.GoToCart();
        cartPage.Checkout();

        checkoutStepOne.EnterDetails("Error", "User", "12345");
        checkoutStepOne.Continue();

        checkoutStepTwo.Finish();

        string currentUrl = _driver.Url;

        Assert.That(currentUrl, Does.Not.Contain("checkout-complete.html"),
            "The Error User was incorrectly allowed to finish the purchase.");

        Assert.That(currentUrl, Does.Contain("checkout-step-two.html"),
            "The user should have remained on the Step Two page due to the error.");
    }
}
