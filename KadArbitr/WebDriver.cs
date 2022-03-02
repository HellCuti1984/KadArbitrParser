using System;
using System.IO;
using System.Diagnostics;
using Polly;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace core.WebDriver
{
    class WebDriver
    {
        const string PATH_TO_CHROME = @"C:\Program Files\Google\Chrome\Application";

        public IWebDriver Driver { get; private set; }

        private readonly string Proxy;
        private readonly string UserAgent;
        private readonly string UserFolder;
        private readonly string CommandLine;
        private readonly string PathToChromeBat;

        private static Process ChromeProcess;

        public WebDriver(string proxy, string userAgent, string userFolder) // --> Экземпляр для простого браузера Chrome со всеми настройками
        {
            Proxy = proxy;
            UserAgent = userAgent;
            UserFolder = $"{Directory.GetCurrentDirectory()}\\local_storages\\{userFolder}";

            string proxy_Server = "";
            string user_Agent = "";

            CommandLine = String.Format("cd /\n cd {0} \nchrome.exe --no-default-browser-check --remote-debugging-port=9222" +
                          " --user-data-dir=\"{1}\"", PATH_TO_CHROME, userFolder);

            if (proxy != "Любой")
                proxy_Server = " --proxy-server=http://" + Proxy;

            if (userAgent != "Любой")
                user_Agent = $" --user-agent=\"{UserAgent}\"\n";

            CommandLine = CommandLine + proxy_Server + user_Agent;

            PathToChromeBat = $"{Directory.GetCurrentDirectory()}\\StartChrome.bat";
            File.WriteAllText(PathToChromeBat, CommandLine);
        }

        public WebDriver(ChromeDriverService ChromeDriverService, ChromeOptions ChromeOptions) // --> Экземпляр тестового ПО
        {
            Driver = new ChromeDriver(ChromeDriverService, ChromeOptions);
        }

        IWebDriver InitializeSelenDriver(ChromeDriverService chromeDriverService, ChromeOptions chromeOptions)
        {
            IWebDriver webDriver = new ChromeDriver(chromeDriverService, chromeOptions);
            return Driver = webDriver;
        }

        public IWebDriver AttachToChrome()
        {
            try
            {
                ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
                chromeDriverService.HideCommandPromptWindow = true; //Скрываем консоль от Selenium

                ChromeOptions options = new ChromeOptions();
                options.DebuggerAddress = "127.0.0.1:9222";

                // Using Polly library: https://github.com/App-vNext/Polly
                // Polly probably isn't needed in a single scenario like this, but can be useful in a broader automation project
                // Once we attach to Chrome with Selenium, use a WebDriverWait implementation
                var policy = Policy
                  .Handle<InvalidOperationException>()
                  .WaitAndRetry(10, t => TimeSpan.FromSeconds(1));

                policy.Execute(() =>
                {
                    try
                    {
                        Driver = new ChromeDriver(chromeDriverService, options);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                });

                return Driver;
            }
            catch (Exception ex)
            {
            }

            return Driver;
        }

        public void StartChromeProcess()
        {
            Process chromeProcess = new Process();
            chromeProcess.StartInfo.FileName = PathToChromeBat;
            chromeProcess.Start();

            ChromeProcess = chromeProcess;
        }

        public static void KillChromeProcess()
        {
            if (ChromeProcess != null)
                ChromeProcess.Kill();
        }

        public IWebDriver StartAndAtachToChrome()
        {
            StartChromeProcess();
            return AttachToChrome();
        }

        public static void TerminateChrome()
        {
            try
            {
                var proc = Process.GetProcessesByName("chrome");
                for (int i = 0; i < proc.Length; i++)
                    proc[i].Kill();
            }
            catch (Exception exc)
            {
                
            }
        }
    }
}