namespace XeroServices.WebDriver
{
    public class WebDriverSettings
    {
        public Browser Browser { get; set; } = Browser.Chrome;
        public bool ShowImages { get; set; } = true;
        public bool HeadlessBrowser { get; set; } = false;
        public int CrawlerLifetimeMinutes { get; set; } = 1200;

        public int BrowserSizeWidth { get; set; } = 1400;
        public int BrowserSizeHeight { get; set; } = 1024;

        public int FindElementTimeoutMilliseconds { get; set; } = 0;
        public int PageLoadTimeoutMilliseconds { get; set; } = 0;
    }
}
