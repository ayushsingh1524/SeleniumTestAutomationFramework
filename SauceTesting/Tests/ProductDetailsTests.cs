using SauceTesting.Pages;

namespace SauceTesting.Tests;

[TestFixture, Order(5)]
public class ProductDetailsTest : BaseTest
{
    private LoginPage loginPage;
    private InventoryPage inventoryPage;
    private ProductDetailsPage detailsPage;

    [SetUp]
    public void PageSetup()
    {
        loginPage = new LoginPage(_driver);
        inventoryPage = new InventoryPage(_driver);
        detailsPage = new ProductDetailsPage(_driver);

        loginPage.Login(StandardUser, DefaultPassword);
        inventoryPage.ResetAppState();
    }

    [Test]
    public void Navigation_ClickItem_ShouldNavigateToDetailsPage()
    {
        string itemName = "Sauce Labs Backpack";
        inventoryPage.OpenProductDetails(itemName);

        Assert.That(detailsPage.GetProductTitle(), Is.EqualTo(itemName));
    }

    [Test]
    public void DataConsistency_PriceOnInventory_ShouldMatchPriceOnDetails()
    {
        decimal inventoryPrice = inventoryPage.GetItemPrices()[0]; 
        string itemName = "Sauce Labs Backpack";

        inventoryPage.OpenProductDetails(itemName);
        decimal detailsPrice = detailsPage.GetProductPrice();

        Assert.That(detailsPrice, Is.EqualTo(inventoryPrice), 
            "Price on Details page matches Inventory page");
    }

    [Test]
    public void Actions_AddToCartFromDetails_ShouldUpdateCart()
    {
        inventoryPage.OpenProductDetails("Sauce Labs Backpack");
        
        detailsPage.ClickAddToCart();

        Assert.That(detailsPage.IsRemoveButtonVisible(), Is.True, "Button text did not change to Remove");
        Assert.That(inventoryPage.GetCartItemCount(), Is.EqualTo(1), "Cart badge did not update");
    }

    [Test]
    public void Navigation_BackButton_ShouldReturnToInventory()
    {
        inventoryPage.OpenProductDetails("Sauce Labs Bike Light");
        detailsPage.BackToProducts();

        Assert.That(inventoryPage.GetPageTitle(), Is.EqualTo("Products"));
    }
}