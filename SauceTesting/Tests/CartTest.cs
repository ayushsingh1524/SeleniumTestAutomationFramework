using NUnit.Framework;
using SauceTesting.Pages;
namespace SauceTesting.Tests;

[TestFixture, Order(3)]
public class CartTest : BaseTest
{
    private LoginPage loginPage;
    private InventoryPage inventoryPage;
    private CartPage cartPage;

    [SetUp]
    public void PageSetup()
    {
        loginPage = new LoginPage(_driver);
        inventoryPage = new InventoryPage(_driver);
        cartPage = new CartPage(_driver);

        loginPage.Login(StandardUser, DefaultPassword);
        inventoryPage.ResetAppState();
    }

    [Test]
    public void Cart_ItemsAddedInInventory_ShouldAppearInCart()
    {
        string item1 = "Sauce Labs Backpack";
        string item2 = "Sauce Labs Bike Light";

        inventoryPage.AddToCart(item1);
        inventoryPage.AddToCart(item2);
        inventoryPage.GoToCart();

        List<string> cartItems = cartPage.GetCartItemNames();

        Assert.That(cartItems, Contains.Item(item1));
        Assert.That(cartItems, Contains.Item(item2));
        Assert.That(cartItems.Count, Is.EqualTo(2));
    }

    [Test]
    public void Cart_RemoveItem_ShouldUpdateList()
    {
        inventoryPage.AddToCart("Sauce Labs Backpack");
        inventoryPage.GoToCart();

        cartPage.RemoveFirstItem();

        Assert.That(cartPage.GetCartItemNames(), Is.Empty, "Cart should be empty after removing the only item");
    }

    [Test]
    public void Cart_ContinueShopping_ShouldReturnToInventory()
    {
        inventoryPage.GoToCart();
        cartPage.ContinueShopping();

        Assert.That(inventoryPage.GetPageTitle(), Is.EqualTo("Products"));
    }

    [Test]
    public void Cart_RemoveItem_And_ContinueShopping_ShouldResetInventoryButtonState()
    {
        string itemName = "Sauce Labs Backpack";
        inventoryPage.AddToCart(itemName);

        Assert.That(inventoryPage.GetCartItemCount(), Is.EqualTo(1));

        inventoryPage.GoToCart();
        cartPage.RemoveFirstItem();
        cartPage.ContinueShopping();

        Assert.That(inventoryPage.GetPageTitle(), Is.EqualTo("Products"));

        Assert.That(inventoryPage.GetCartItemCount(), Is.EqualTo(0), "Cart badge was not cleared.");

        string actualButtonText = inventoryPage.GetProductButtonText(itemName);
        Assert.That(actualButtonText, Is.EqualTo("Add to cart"),
            $"The button state for '{itemName}' did not reset after removing it from the cart.");
    }
}
