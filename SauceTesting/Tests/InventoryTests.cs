using SauceTesting.Pages;
using SauceTesting.Enums;
namespace SauceTesting.Tests;

[TestFixture, Order(2)]
public class InventoryTest : BaseTest
{
    private LoginPage loginPage;
    private InventoryPage inventoryPage;
    private ProductDetailsPage productDetailsPage;

    [SetUp]
    public void PageSetup()
    {
        loginPage = new LoginPage(_driver);
        inventoryPage = new InventoryPage(_driver);
        productDetailsPage = new ProductDetailsPage(_driver);

        loginPage.Login(StandardUser, DefaultPassword);



        // inventoryPage.ResetAppState();
    }

    [Test]
    public void UI_VerifyDefaultSortOption_ShouldBeNameAtoZ()
    {
        List<string> currentNames = inventoryPage.GetItemNames();
        List<string> sortedNames = currentNames.OrderBy(x => x).ToList();

        Assert.That(currentNames, Is.EqualTo(sortedNames), "Default sorting was not A to Z");
    }

    [Test]
    public void Sort_PriceLowToHigh_ShouldSortItemsByPriceAscending()
    {
        inventoryPage.SortBy(SortOptions.PriceLowHigh);

        List<decimal> actualPrices = inventoryPage.GetItemPrices();

        List<decimal> expectedPrices = actualPrices.OrderBy(x => x).ToList();

        Assert.That(actualPrices, Is.EqualTo(expectedPrices), "Items were not sorted by Price (Low to High)");
    }

    [Test]
    public void Sort_NameZtoA_ShouldSortItemsAlhpabeticlyDescending()
    {
        inventoryPage.SortBy(SortOptions.NameZtoA);

        List<string> actualNames = inventoryPage.GetItemNames();

        List<string> expectedNames = actualNames.OrderByDescending(x => x).ToList();

        Assert.That(actualNames, Is.EqualTo(expectedNames), "Items were not sorted by Name (Z to A)");
    }

    [Test]
    public void Cart_AddAndRemoveItems_ShouldUpdateBadgeCount()
    {
        Assert.That(inventoryPage.GetCartItemCount(), Is.EqualTo(0));

        string firstItemName = inventoryPage.GetItemNames().First();
        string secondItemName = inventoryPage.GetItemNames()[1];


        inventoryPage.AddToCart(firstItemName);
        inventoryPage.AddToCart(secondItemName);

        Assert.That(inventoryPage.GetCartItemCount(), Is.EqualTo(2), "Cart badge did not update to 2");

        inventoryPage.RemoveFromCart(firstItemName);

        Assert.That(inventoryPage.GetCartItemCount(), Is.EqualTo(1), "Cart badge did not decrement after removal");
    }

    [Test]
    public void ProblemUser_ShouldSeeBrokenImages()
    {
        inventoryPage.Logout();

        loginPage.Login(ProblemUser, DefaultPassword);

        string imageSrc = inventoryPage.GetImageSource(0);

        bool isCorrectImage = imageSrc.Contains("sauce-backpack");

        Assert.That(isCorrectImage, Is.False,
            "Problem User should see a broken or incorrect image, but saw the correct backpack image.");
        inventoryPage.Logout();
    }
    [Test]
    public void Sort_State_ShouldReturnToDefault_AfterNavigation()
    {
        string defaultOption = inventoryPage.GetActiveSortOption();
        string sortOption = SortOptions.PriceHighLow;
        inventoryPage.SortBy(sortOption);
        string firstItemName = inventoryPage.GetItemNames().First();

        inventoryPage.OpenProductDetails(firstItemName);

        productDetailsPage.BackToProducts();
        string newOption = inventoryPage.GetActiveSortOption();

        Assert.That(defaultOption, Is.EqualTo(newOption),
            "Sort dropdown reset to default after navigating back from product details.");

    }
}
