using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace GameStore.E2ETests
{
    public class ClientCredentialsTests : IDisposable
    {
        private readonly IWebDriver _driver;

        public ClientCredentialsTests()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            _driver = new ChromeDriver(options);
        }

        [Fact]
        public void RegisterPage_ShouldLoadProperly()
        {
            _driver.Navigate().GoToUrl("https://localhost:7194");
            Thread.Sleep(2000);

            var registerNavLink = _driver.FindElement(By.XPath("//a[contains(text(), 'Register')]"));
            registerNavLink.Click();
            Thread.Sleep(2000);

            var title = _driver.FindElement(By.XPath("//h3[text()='Register new account']"));
            Assert.NotNull(title);

            var usernameInput = _driver.FindElement(By.XPath("//input[@id='username']"));
            usernameInput.SendKeys("TestUser123");

            var emailInput = _driver.FindElement(By.XPath("//input[@id='email']"));
            emailInput.SendKeys("test@example.com");

            var passwordInput = _driver.FindElement(By.XPath("//input[@id='password']"));
            passwordInput.SendKeys("SecurePassword123!");

            var firstNameInput = _driver.FindElement(By.XPath("//input[@id='firstName']"));
            firstNameInput.SendKeys("Test");

            var lastNameInput = _driver.FindElement(By.XPath("//input[@id='lastName']"));
            lastNameInput.SendKeys("User");

            var registerButton = _driver.FindElement(By.XPath("//button[@type='submit']"));
            registerButton.Click();

            Thread.Sleep(3000);

            Assert.Contains("localhost", _driver.Url);
        }

        [Fact]
        public void Login_ShouldSucceed_WithValidCredentials()
        {
            _driver.Navigate().GoToUrl("https://localhost:7194");
            Thread.Sleep(2000);

            var loginNavLink = _driver.FindElement(By.XPath("//a[contains(text(), 'Login')]"));
            loginNavLink.Click();
            Thread.Sleep(2000);

            var usernameInput = _driver.FindElement(By.XPath("//input[@id='username']"));
            usernameInput.SendKeys("TestUser123");

            var passwordInput = _driver.FindElement(By.XPath("//input[@id='password']"));
            passwordInput.SendKeys("SecurePassword123!");

            var loginButton = _driver.FindElement(By.XPath("//button[@type='submit']"));
            loginButton.Click();

            Thread.Sleep(1000);

            var errorMessages = _driver.FindElements(By.XPath("//*[contains(text(), 'Invalid username or password')]"));
            Assert.Empty(errorMessages);

            Assert.Contains("localhost", _driver.Url);
        }

        [Fact]
        public void Login_ShouldFail_WithInvalidCredentials()
        {
            _driver.Navigate().GoToUrl("https://localhost:7194");
            Thread.Sleep(2000);

            var loginNavLink = _driver.FindElement(By.XPath("//a[contains(text(), 'Login')]"));
            loginNavLink.Click();
            Thread.Sleep(2000);

            var usernameInput = _driver.FindElement(By.XPath("//input[@id='username']"));
            usernameInput.SendKeys("WrongUser");

            var passwordInput = _driver.FindElement(By.XPath("//input[@id='password']"));
            passwordInput.SendKeys("WrongPassword!");

            var loginButton = _driver.FindElement(By.XPath("//button[@type='submit']"));
            loginButton.Click();

            Thread.Sleep(1000);

            var errorMessage = _driver.FindElement(By.XPath("//*[contains(text(), 'User with username WrongUser not found!')]"));
            Assert.NotNull(errorMessage);
        }


        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}