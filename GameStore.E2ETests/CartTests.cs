using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace GameStore.E2ETests
{
    public class CartTests : IDisposable
    {
        private readonly IWebDriver _driver;

        public CartTests()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            _driver = new ChromeDriver(options);
        }

        [Fact]
        public void AddGameToCart_ShouldShowConfirmationMessage()
        {
            _driver.Navigate().GoToUrl("https://localhost:7194");
            Thread.Sleep(2000);

            var catalogNavLink = _driver.FindElement(By.XPath("//a[contains(text(), 'Game Catalog')]"));
            catalogNavLink.Click();
            Thread.Sleep(1000);

            var firstGameCard = _driver.FindElement(By.XPath("(//div[contains(@class, 'card')])[1]"));
            firstGameCard.Click();
            Thread.Sleep(1000);

            var addToCartButton = _driver.FindElement(By.XPath("//button[contains(text(), 'Add to Cart')]"));
            addToCartButton.Click();
            Thread.Sleep(1000);

            var confirmation = _driver.FindElement(By.XPath("//*[contains(text(), 'Added to cart!')]"));
            Assert.NotNull(confirmation);
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}
