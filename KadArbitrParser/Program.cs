using OpenQA.Selenium;

namespace KadArbitrParser
{
    class Program
    {
        static IWebDriver driver { get; set; } = null;

        static IWebElement fromJsElem(string query)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            return (IWebElement)js.ExecuteScript(query);
        }

        static void Main(string[] args)
        {
            driver = new core.WebDriver.WebDriver("Любой", "", "Тест").StartAndAtachToChrome();

            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://kad.arbitr.ru/");
            Thread.Sleep(1500);

            //КЛИК ВЫБОРА СВЕРДЛОВСКОГО СУДА
            IWebElement courtInput = driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div[1]/dl/dd/div[3]/div/span/label/input"));
            courtInput.Click();
            courtInput.SendKeys("АС Свердловской области");
            Thread.Sleep(500);

            //КЛИК ВВОДЫ ДАТЫ
            IWebElement firstDate = driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div[1]/dl/dd/div[5]/label[1]/input"));
            IWebElement lastDate = driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div[1]/dl/dd/div[5]/label[2]/input"));

            firstDate.Click();
            firstDate.SendKeys("01.01.2022");
            lastDate.Click();
            lastDate.SendKeys("30.01.2022");

            var start_btn = driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div[1]/dl/dd/div[7]/div[1]/div/button"));
            start_btn.Click();

            Thread.Sleep(2000);

            //КЛИК ВЫБОРА "БАНКРОТНЫЕ"
            IWebElement bankruptcy = driver.FindElement(By.CssSelector(".bankruptcy"));
            bankruptcy.Click();
            Thread.Sleep(1500);

            List<string> curText = new List<string>();
            var respondentElem = driver.FindElements(By.CssSelector(".respondent"));
            for (int i = 0; i < respondentElem.Count; i++)
                curText.Add(fromJsElem($"return document.querySelectorAll('.respondent .js-rolloverHtml')[{i}].textContent").Text);

            for(int i = 0; i < curText.Count; i++)
                Console.WriteLine(curText[i]);

            Thread.Sleep(1500);

            driver.Quit();
        }
    }
}

