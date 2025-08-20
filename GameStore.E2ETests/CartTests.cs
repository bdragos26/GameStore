using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace GameStore.E2ETests
{
    public class CartTests : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public CartTests()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            _driver = new ChromeDriver(options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        [Fact]
        public void AddGameToCart_ShouldShowConfirmationMessage()
        {
            _driver.Navigate().GoToUrl("https://localhost:7194");
            Thread.Sleep(2000);

            var catalogNavLink = _driver.FindElement(By.XPath("//a[@href='/gameList']"));
            catalogNavLink.Click();
            Thread.Sleep(1000);

            var firstGameCard = _driver.FindElement(By.XPath("(//div[contains(@class, 'card')])[1]"));
            firstGameCard.Click();
            Thread.Sleep(1000);

            var addToCartButton = _driver.FindElement(By.CssSelector("button.btn-success"));
            addToCartButton.Click();
            Thread.Sleep(1000);

            var confirmation = _driver.FindElement(By.CssSelector("span.text-success"));
            Assert.True(confirmation.Displayed);
        }

        [Fact]
        public void CartWorkflow_AddUpdateRemoveItems_WithSleep()
        {
            _driver.Navigate().GoToUrl("https://localhost:7194");
            Thread.Sleep(2000);

            var catalogNavLink = _driver.FindElement(By.XPath("//a[@href='/gameList']"));
            catalogNavLink.Click();
            Thread.Sleep(1000);

            var firstGameCard = _driver.FindElement(By.XPath("(//div[contains(@class, 'card')])[1]"));
            firstGameCard.Click();
            Thread.Sleep(1000);

            var addToCartButton = _driver.FindElement(By.CssSelector("button.btn-success"));
            addToCartButton.Click();
            Thread.Sleep(1000);

            var confirmation = _driver.FindElement(By.CssSelector("span.text-success"));
            Assert.True(confirmation.Displayed);
            Thread.Sleep(1000);

            var cartIcon = _driver.FindElement(By.XPath("//a[@href='/cart']"));
            cartIcon.Click();
            Thread.Sleep(1000);

            var cartHeader = _driver.FindElement(By.XPath("//h3[contains(text(), 'My Cart')]"));
            Thread.Sleep(1000);

            var quantityInput = _driver.FindElement(By.XPath("//input[@type='number']"));
            quantityInput.Clear();
            Thread.Sleep(500);
            quantityInput.SendKeys("3");
            Thread.Sleep(1000);

            var tableHeader = _driver.FindElement(By.XPath("//th[contains(text(), 'Game')]"));
            tableHeader.Click();
            Thread.Sleep(1000);

            var totalCell = _driver.FindElement(By.XPath("(//td[contains(text(), '$')])[2]"));
            Assert.NotEqual("$0.00", totalCell.Text);
            Thread.Sleep(1000);

            var deleteButton = _driver.FindElement(By.XPath("//button[contains(text(), 'Delete')]"));
            deleteButton.Click();
            Thread.Sleep(1000);

            var emptyCartMessage = _driver.FindElement(By.XPath("//p[contains(text(), 'Cart empty')]"));
            Assert.NotNull(emptyCartMessage);
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}
