using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace GameStore.E2ETests
{
    public class RegisterPageTests
    {
        [Fact]
        public void RegisterPage_ShouldLoadProperly()
        {
            using var driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://localhost:7194/register");
            Thread.Sleep(5000);

            var title = driver.FindElement(By.XPath("//h3[text()='Register new account']"));
            Assert.NotNull(title);

            var usernameInput = driver.FindElement(By.XPath("//input[@id='username']"));
            usernameInput.SendKeys("TestUser123");

            var emailInput = driver.FindElement(By.XPath("//input[@id='email']"));
            emailInput.SendKeys("test@example.com");

            var passwordInput = driver.FindElement(By.XPath("//input[@id='password']"));
            passwordInput.SendKeys("SecurePassword123!");

            var firstNameInput = driver.FindElement(By.XPath("//input[@id='firstName']"));
            firstNameInput.SendKeys("Test");

            var lastNameInput = driver.FindElement(By.XPath("//input[@id='lastName']"));
            lastNameInput.SendKeys("User");

            var registerButton = driver.FindElement(By.XPath("//button[@type='submit']"));
            registerButton.Click();

            Thread.Sleep(3000);

            Assert.Contains("localhost", driver.Url);
        }
    }
}