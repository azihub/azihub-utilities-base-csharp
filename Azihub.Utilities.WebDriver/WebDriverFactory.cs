using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XeroServices.WebDriver
{
    public static class WebDriverFactory
    {

        public static IWebDriver Create(WebDriverSettings settings = null, string userAgent = null)
        {
            if (settings is null)
                settings = new WebDriverSettings();

            ChromeOptions chromeOptions = new ChromeOptions();

            chromeOptions.AddUserProfilePreference("profile.default_content_settings.popups", 0);
            chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.automatic_downloads", 1);
            chromeOptions.AddUserProfilePreference("download.prompt_for_download", false);
            chromeOptions.AddUserProfilePreference("safe-browsing.enabled", false);
            chromeOptions.AddUserProfilePreference("safebrowsing", false);
            chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
            chromeOptions.AddUserProfilePreference("credentials_enable_service", false);
            chromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);

            // set user agent
            if (!string.IsNullOrEmpty(userAgent))
            {
                chromeOptions.AddArgument($"--user-agent={userAgent}");
            }

            // load single file extension in project
            //chromeOptions.AddExtension("./SingleFile.crx");

            chromeOptions.AddExcludedArguments(new List<string>() { "enable-automation" });
            chromeOptions.AddArgument("--disable-blink-features");
            chromeOptions.AddArgument("--disable-blink-features=AutomationControlled");
            // chromeOptions.AddArgument("--disable-extensions"); // disabling extensions
            // chromeOptions.AddArgument("--disable-gpu"); // applicable to windows os only
            // chromeOptions.AddArgument("--disable-dev-shm-usage"); // overcome limited resource problems
            // chromeOptions.PageLoadStrategy = PageLoadStrategy.Eager;

            if (!settings.ShowImages)
            {
                chromeOptions.AddUserProfilePreference("profile.default_content_settings", 2);
            }

            if (settings.HeadlessBrowser)
            {
                chromeOptions.AddArgument("--headless");
            }

            IWebDriver webDriver;
            
            // chromeOptions.AddUserProfilePreference("download.default_directory", downloadDirectory);
            webDriver = new ChromeDriver(chromeOptions); ;
            //webDriver.SetWindowSize(new Size(1150, 900));
            webDriver.SetWindowSize(new Size(settings.BrowserSizeWidth, settings.BrowserSizeHeight));


            return webDriver;
        }
    }
}
