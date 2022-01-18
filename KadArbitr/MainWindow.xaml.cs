using System;
using System.Windows;
using System.Threading;
using System.Collections.Generic;
using OpenQA.Selenium;
using KadArbitrParser;

namespace KadArbitr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        static IWebDriver driver { get; set; } = null;

        static IWebElement fromJsElem(string query)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            return (IWebElement)js.ExecuteScript(query);
        }

        private void StartParsing_Click(object sender, RoutedEventArgs e)
        {
            driver = new core.WebDriver.WebDriver("Любой", "", "Тест").StartAndAtachToChrome();

            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://kad.arbitr.ru/");
            Thread.Sleep(1500);

            //КЛИК ВЫБОРА СУДА
            IWebElement courtInput = driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div[1]/dl/dd/div[3]/div/span/label/input"));
            courtInput.Click();
            courtInput.SendKeys(CourtCB.Text);
            Thread.Sleep(500);

            //КЛИК ВВОДА ДАТЫ
            IWebElement firstDate = driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div[1]/dl/dd/div[5]/label[1]/input"));
            IWebElement lastDate = driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div[1]/dl/dd/div[5]/label[2]/input"));

            firstDate.Click();
            firstDate.SendKeys(FirstDate.Text);
            lastDate.Click();
            lastDate.SendKeys(LastDate.Text);

            var start_btn = driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div[1]/dl/dd/div[7]/div[1]/div/button"));
            start_btn.Click();
            Thread.Sleep(2000);

            //КЛИК ВЫБОРА "БАНКРОТНЫЕ"
            IWebElement bankruptcy = driver.FindElement(By.CssSelector(".bankruptcy"));
            bankruptcy.Click();
            Thread.Sleep(1000);

            List<KadModel> kads = new List<KadModel>();
            for (int p = 0; p < 40; p++)
            {
                try
                {
                    Thread.Sleep(1000);
                    List<string> curText = new List<string>();
                    var respondentElem = driver.FindElements(By.CssSelector(".respondent"));
                    for (int i = 0; i < respondentElem.Count; i++)
                    {
                        IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                        string text = (string)js.ExecuteScript($"return document.querySelectorAll('.respondent .js-rolloverHtml')[{i}].textContent");
                        curText.Add(text);
                    }

                    for (int i = 0; i < curText.Count; i++)
                    {
                        string text = curText[i].Replace("\r\n                                        ", "$");

                        string[] kadStr = text.Split('$');

                        KadModel kad = new KadModel();
                        try
                        {
                            kad.Name = kadStr[0];
                            kad.Adress = kadStr[1];
                            kad.INN = kadStr[3];
                        }
                        catch (Exception ex)
                        {
                            kad.Name = kadStr[0];
                            kad.Adress = kadStr[1];
                            kad.INN = kadStr[2];
                        }

                        kads.Add(kad);
                    }

                    driver.FindElement(By.CssSelector(".rarr")).Click();
                }
                catch (Exception ex)
                {
                    break;
                }
            }

            Excells.Save(kads, fileName: CourtCB.Text);

            driver.Quit();
            core.WebDriver.WebDriver.TerminateChrome();
        }
    }
}
