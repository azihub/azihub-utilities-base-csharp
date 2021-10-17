using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using Size = System.Drawing.Size;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using System.Drawing.Imaging;
using Azihub.Utilities.Base.Logger;

namespace XeroServices.WebDriver
{
    public static class WebDriverExtensions
    {
        private static readonly ILogger Logger = AppLogger.CreateLogger(typeof(WebDriverExtensions));

        #region DoOperationWite
        public static TResult DoOperation<TResult>(this IWebDriver webDriver, Func<IWebDriver, TResult> operation, int timeoutMilliseconds = 2000, int retryCount = 2, Action<Exception> onError = null)
        {
            int tryCount = 0;
            while (tryCount++ < retryCount)
            {
                try
                {
                    return operation(webDriver);
                }
                catch (NotFoundException ex)
                {
                    onError?.Invoke(new NotFoundException($"Selenium operation failed after {retryCount} retry", ex));
                }
                if (tryCount < retryCount)
                {
                    Thread.Sleep(timeoutMilliseconds);
                }
            }
            throw new NotFoundException($"Selenium operation failed after {retryCount} retry");
        }

        public static async Task<TResult> DoOperationAsync<TResult>(this IWebDriver webDriver, Func<IWebDriver, Task<TResult>> operation, int timeoutMilliseconds = 2000, int retryCount = 2, Action<Exception> onError = null)
        {
            int tryCount = 0;
            while (tryCount++ < retryCount)
            {
                try
                {
                    return await operation(webDriver);
                }
                catch (Exception ex)
                {
                    onError?.Invoke(new Exception($"Selenium operation failed after {retryCount} retry", ex));
                }
                if (tryCount < retryCount)
                {
                    Thread.Sleep(timeoutMilliseconds);
                }
            }
            throw new Exception("Selenium operation failed");
        }

        public static IWebElement GetRootElement(this IWebDriver webDriver, IWebElement element)
        {
            IWebElement rootElement = (IWebElement)((IJavaScriptExecutor)webDriver).ExecuteScript("return arguments[0].shadowRoot", element);
            return rootElement;
        }

        public static WebDriverWait GetWait(this IWebDriver webDriver, int timeoutMilliseconds = 2000, int intervalMilliseconds = 500, params Type[] exceptionTypes)
        {
            WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromMilliseconds(timeoutMilliseconds))
            {
                PollingInterval = TimeSpan.FromMilliseconds(intervalMilliseconds)
            };
            wait.IgnoreExceptionTypes(exceptionTypes);
            return wait;
        }

        public static TResult DoWait<TResult>(this IWebDriver webDriver, Func<IWebDriver, TResult> operation, int timeoutMilliseconds, int intervalMilliseconds = 500, params Type[] exceptionTypes)
        {
            WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromMilliseconds(timeoutMilliseconds))
            {
                PollingInterval = TimeSpan.FromMilliseconds(intervalMilliseconds)
            };
            wait.IgnoreExceptionTypes(exceptionTypes);
            return wait.Until(operation);
        }

        public static Task<TResult> DoWaitAsync<TResult>(this IWebDriver webDriver, Func<IWebDriver, Task<TResult>> operation, int timeoutMilliseconds, int intervalMilliseconds = 500, params Type[] exceptionTypes)
        {
            WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromMilliseconds(timeoutMilliseconds))
            {
                PollingInterval = TimeSpan.FromMilliseconds(intervalMilliseconds)
            };
            wait.IgnoreExceptionTypes(exceptionTypes);
            return wait.Until(operation);
        }
        #endregion

        #region ExpectedConditions
        public static IAlert AlertIsPresent(this IWebDriver webDriver)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent()(webDriver);
        }

        public static bool AlertState(this IWebDriver webDriver, bool state)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.AlertState(state)(webDriver);
        }

        public static IWebElement ElementExists(this IWebDriver webDriver, By path)
        {
            var elements = webDriver.FindElements(path);
            return (elements.Count >= 1) ? elements.First() : null;
        }

        public static IWebElement WaitForElementIsVisible(this IWebDriver webDriver, By path, int timeoutMilliseconds = 20000, int intervalMilliseconds = 500)
        {
            WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromMilliseconds(timeoutMilliseconds))
            {
                PollingInterval = TimeSpan.FromMilliseconds(intervalMilliseconds)
            };

            try
            {
                return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(path));
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Element is not visible by: {path}");
            }

            throw new NotFoundException($"Element was not found by {path}");

            // return SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(path)(webDriver);
        }

        public static IWebElement ElementIsVisible(this IWebDriver webDriver, By path)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(path)(webDriver);
        }

        public static bool ElementSelectionStateToBe(this IWebDriver webDriver, By path, bool selected)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.ElementSelectionStateToBe(path, selected)(webDriver);
        }

        public static bool ElementSelectionStateToBe(this IWebDriver webDriver, IWebElement element, bool selected)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.ElementSelectionStateToBe(element, selected)(webDriver);
        }

        public static IWebElement ElementToBeClickable(this IWebDriver webDriver, IWebElement element)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(element)(webDriver);
        }

        public static IWebElement ElementToBeClickable(this IWebDriver webDriver, By path)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(path)(webDriver);
        }

        public static bool ElementToBeSelected(this IWebDriver webDriver, IWebElement element, bool selected)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeSelected(element, selected)(webDriver);
        }

        public static bool ElementToBeSelected(this IWebDriver webDriver, By path)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeSelected(path)(webDriver);
        }

        public static bool ElementToBeSelected(this IWebDriver webDriver, IWebElement element)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeSelected(element)(webDriver);
        }

        public static IWebDriver FrameToBeAvailableAndSwitchToIt(this IWebDriver webDriver, string frameLocator)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.FrameToBeAvailableAndSwitchToIt(frameLocator)(webDriver);
        }

        public static IWebDriver FrameToBeAvailableAndSwitchToIt(this IWebDriver webDriver, By path)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.FrameToBeAvailableAndSwitchToIt(path)(webDriver);
        }

        public static bool InvisibilityOfElementLocated(this IWebDriver webDriver, By path)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(path)(webDriver);
        }

        public static bool InvisibilityOfElementWithText(this IWebDriver webDriver, By path, string text)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementWithText(path, text)(webDriver);
        }

        public static ReadOnlyCollection<IWebElement> PresenceOfAllElementsLocatedBy(this IWebDriver webDriver, By path)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(path)(webDriver);
        }

        public static bool StalenessOf(this IWebDriver webDriver, IWebElement element)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.StalenessOf(element)(webDriver);
        }

        public static bool TextToBePresentInElement(this IWebDriver webDriver, IWebElement element, string text)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElement(element, text)(webDriver);
        }

        public static bool TextToBePresentInElementLocated(this IWebDriver webDriver, By path, string text)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElementLocated(path, text)(webDriver);
        }

        public static bool TextToBePresentInElementValue(this IWebDriver webDriver, By path, string text)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElementValue(path, text)(webDriver);
        }

        public static bool TextToBePresentInElementValue(this IWebDriver webDriver, IWebElement element, string text)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElementValue(element, text)(webDriver);
        }

        public static bool TitleContains(this IWebDriver webDriver, string title)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.TitleContains(title)(webDriver);
        }

        public static bool TitleIs(this IWebDriver webDriver, string title)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.TitleIs(title)(webDriver);
        }

        public static bool UrlContains(this IWebDriver webDriver, string fraction)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.UrlContains(fraction)(webDriver);
        }

        public static bool UrlMatches(this IWebDriver webDriver, string regex)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.UrlMatches(regex)(webDriver);
        }

        public static bool UrlToBe(this IWebDriver webDriver, string url)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.UrlToBe(url)(webDriver);
        }

        public static ReadOnlyCollection<IWebElement> VisibilityOfAllElementsLocatedBy(this IWebDriver webDriver, ReadOnlyCollection<IWebElement> elements)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(elements)(webDriver);
        }

        public static ReadOnlyCollection<IWebElement> VisibilityOfAllElementsLocatedBy(this IWebDriver webDriver, By path)
        {
            return SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(path)(webDriver);
        }
        #endregion

        #region FindElementWait
        public static IWebElement FindElementWait(this IWebDriver webDriver, By path, int timeoutMilliseconds = 2000, int retryCount = 2)
        {
            int tryCount = 0;
            while (tryCount++ < retryCount)
            {
                try
                {
                    return webDriver.FindElement(path);
                }
                catch (NotFoundException)
                {
                    Logger.LogDebug($"Failed to find element: {path} (attempt {tryCount}/{retryCount}");
                    if (tryCount < retryCount)
                    {
                        Thread.Sleep(timeoutMilliseconds);
                    }
                    else
                        throw;
                }
            }
            throw new NotFoundException($"Selenium operation failed after {retryCount} retry");
            //return webDriver.DoOperation(driver => 
            //        driver.FindElement(path), timeoutMilliseconds, retryCount,
            //    ex => Logger.LogError(ex, "Cannot FindElement by {path}", path));
        }

        public static void ScrollToEnd(this IWebDriver webDriver)
        {
            // scroll to end
            ((IJavaScriptExecutor)webDriver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");

        }

        public static void ScrollToStart(this IWebDriver webDriver)
        {
            // scroll to start
            ((IJavaScriptExecutor)webDriver).ExecuteScript("window.scrollTo(0, 0);");
        }

        public static ReadOnlyCollection<IWebElement> FindElementsWait(this IWebDriver webDriver, By path, int timeoutMilliseconds = 2000, int retryCount = 2)
        {
            return webDriver.DoOperation(driver => driver.FindElements(path), timeoutMilliseconds, retryCount,
                ex => Logger.LogError(ex, "Cannot FindElements by {path}", path));
        }

        public static IWebElement FindElementUntil(this IWebDriver webDriver, By path, int timeoutMilliseconds, int intervalMilliseconds = 500, params Type[] exceptionTypes)
        {
            return webDriver.GetWait(timeoutMilliseconds, intervalMilliseconds, exceptionTypes).Until(driver => driver.FindElement(path));
        }

        public static ReadOnlyCollection<IWebElement> FindElementsWait2(this IWebDriver webDriver, By path, int timeoutMilliseconds, int intervalMilliseconds = 500, params Type[] exceptionTypes)
        {
            return webDriver.GetWait(timeoutMilliseconds, intervalMilliseconds, exceptionTypes).Until(driver => driver.FindElements(path));
        }

        public static IWebElement FindElementWait(this IWebDriver webDriver, IWebElement containerElement, By path, int timeoutMilliseconds = 2000, int retryCount = 2)
        {
            return webDriver.DoOperation(_ =>
            {
                IWebElement element = containerElement.FindElement(path);
                if (element == null)
                {
                    throw new NotFoundException($"Cannot FindElement by {path}");
                }

                return element;
            }, timeoutMilliseconds, retryCount, ex => Logger.LogError(ex, "Cannot FindElement by {path}", path));
        }

        public static ReadOnlyCollection<IWebElement> FindElementsWait(this IWebDriver webDriver, IWebElement containerElement, By path, int timeoutMilliseconds, int retryCount = 2)
        {
            return webDriver.DoOperation(_ =>
            {
                ReadOnlyCollection<IWebElement> elements = containerElement.FindElements(path);
                if (elements == null || elements.Count == 0)
                {
                    throw new Exception($"Cannot FindElements by {path}");
                }

                return elements;
            }, timeoutMilliseconds, retryCount, ex => Logger.LogError(ex, "Cannot FindElements by {path}", path));
        }
        #endregion

        #region SetTimeout
        public static void SetFindElementTimeout(this IWebDriver webDriver, int timeoutMiliseconds)
        {
            if (timeoutMiliseconds > 0)
            {
                webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(timeoutMiliseconds);
            }
        }

        public static void SetPageLoadTimeout(this IWebDriver webDriver, int timeoutMiliseconds)
        {
            if (timeoutMiliseconds > 0)
            {
                webDriver.Manage().Timeouts().PageLoad = TimeSpan.FromMilliseconds(timeoutMiliseconds);
            }
        }
        #endregion

        #region Window
        /// <summary>
        /// Gets or sets the position of the browser window relative to the upper-left corner
        /// of the screen.
        /// When setting this property, it should act as the JavaScript window.moveTo() method.
        /// </summary>
        /// <param name="webDriver"></param>
        /// <param name="position"></param>
        public static void SetWindowPosition(this IWebDriver webDriver, Point position)
        {
            webDriver.Manage().Window.Position = position;
        }

        /// <summary>
        /// Gets or sets the size of the outer browser window, including title bars and window
        /// borders.
        /// When setting this property, it should act as the JavaScript window.resizeTo()
        /// method.
        /// </summary>
        /// <param name="webDriver"></param>
        /// <param name="size"></param>
        public static void SetWindowSize(this IWebDriver webDriver, Size size)
        {
            webDriver.Manage().Window.Size = size;
        }

        /// <summary>
        /// Sets the current window to full screen if it is not already in that state.
        /// </summary>
        /// <param name="webDriver"></param>
        public static void SetWindowFullScreen(this IWebDriver webDriver)
        {
            webDriver.Manage().Window.FullScreen();
        }

        /// <summary>
        /// Maximizes the current window if it is not already maximized.
        /// </summary>
        /// <param name="webDriver"></param>
        public static void SetWindowMaximize(this IWebDriver webDriver)
        {
            webDriver.Manage().Window.Maximize();
        }

        /// <summary>
        /// Minimizes the current window if it is not already maximized.
        /// </summary>
        /// <param name="webDriver"></param>
        public static void SetWindowMinimize(this IWebDriver webDriver)
        {
            webDriver.Manage().Window.Minimize();
        }
        #endregion

        #region Cookie
        /// <summary>
        /// Gets all cookies defined for the current page.
        /// </summary>
        /// <param name="webDriver"></param>
        /// <returns></returns>
        public static ReadOnlyCollection<Cookie> GetCookies(this IWebDriver webDriver)
        {
            return webDriver.Manage().Cookies.AllCookies;
        }

        /// <summary>
        /// Adds a cookie to the current page.
        /// </summary>
        /// <param name="webDriver"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static void AddCookie(this IWebDriver webDriver, Cookie cookie)
        {
            webDriver.Manage().Cookies.AddCookie(cookie);
        }

        /// <summary>
        /// Deletes all cookies from the page.
        /// </summary>
        /// <param name="webDriver"></param>
        /// <returns></returns>
        public static void DeleteAllCookies(this IWebDriver webDriver)
        {
            webDriver.Manage().Cookies.DeleteAllCookies();
        }

        /// <summary>
        /// Deletes the specified cookie from the page.
        /// </summary>
        /// <param name="webDriver"></param>
        /// <returns></returns>
        public static void DeleteCookie(this IWebDriver webDriver, Cookie cookie)
        {
            webDriver.Manage().Cookies.DeleteCookie(cookie);
        }

        /// <summary>
        /// Deletes the cookie with the specified name from the page.
        /// </summary>
        /// <param name="name"></param>
        public static void DeleteCookieNamed(this IWebDriver webDriver, string name)
        {
            webDriver.Manage().Cookies.DeleteCookieNamed(name);
        }

        /// <summary>
        /// Gets a cookie with the specified name.
        /// </summary>
        /// <param name="webDriver"></param>
        /// <returns></returns>
        public static Cookie GetCookieNamed(this IWebDriver webDriver, string name)
        {
            return webDriver.Manage().Cookies.GetCookieNamed(name);
        }
        #endregion

        #region NavigateWait
        /// <summary>
        /// Move back a single entry in the browser's history.
        /// </summary>
        /// <param name="webDriver"></param>
        /// <param name="timeoutMilliseconds"></param>
        /// <param name="intervalMilliseconds"></param>
        public static void BackWait(this IWebDriver webDriver, int timeoutMilliseconds = 4000, int intervalMilliseconds = 500)
        {
            webDriver.Navigate().Back();
            webDriver.WaitForReadyState(timeoutMilliseconds, intervalMilliseconds);
        }

        /// <summary>
        /// Move a single "item" forward in the browser's history.
        /// Does nothing if we are on the latest page viewed.
        /// </summary>
        /// <param name="webDriver"></param>
        /// <param name="timeoutMilliseconds"></param>
        /// <param name="intervalMilliseconds"></param>
        public static void ForwardWait(this IWebDriver webDriver, int timeoutMilliseconds = 4000, int intervalMilliseconds = 500)
        {
            webDriver.Navigate().Forward();
            webDriver.WaitForReadyState(timeoutMilliseconds, intervalMilliseconds);
        }

        /// <summary>
        /// Load a new web page in the current browser window.
        /// Calling the OpenQA.Selenium.INavigation.GoToUrl(System.String) method will load
        /// a new web page in the current browser window. This is done using an HTTP GET
        /// operation, and the method will block until the load is complete. This will follow
        /// redirects issued either by the server or as a meta-redirect from within the returned
        /// HTML. Should a meta-redirect "rest" for any duration of time, it is best to wait
        /// until this timeout is over, since should the underlying page change while your
        /// test is executing the results of future calls against this interface will be
        /// against the freshly loaded page.
        /// </summary>
        /// <param name="webDriver"></param>
        /// <param name="url"></param>
        /// <param name="timeoutMilliseconds"></param>
        /// <param name="intervalMilliseconds"></param>
        public static void GoToUrlWait(this IWebDriver webDriver, string url, int timeoutMilliseconds = 60000, int intervalMilliseconds = 500)
        {
            webDriver.Navigate().GoToUrl(url);
            webDriver.WaitForReadyState(timeoutMilliseconds, intervalMilliseconds);
        }

        /// <summary>
        /// Load a new web page in the current browser window.
        /// Calling the OpenQA.Selenium.INavigation.GoToUrl(System.Uri) method will load
        /// a new web page in the current browser window. This is done using an HTTP GET
        /// operation, and the method will block until the load is complete. This will follow
        /// redirects issued either by the server or as a meta-redirect from within the returned
        /// HTML. Should a meta-redirect "rest" for any duration of time, it is best to wait
        /// until this timeout is over, since should the underlying page change while your
        /// test is executing the results of future calls against this interface will be
        /// against the freshly loaded page.
        /// </summary>
        /// <param name="webDriver"></param>
        /// <param name="url"></param>
        /// <param name="timeoutMilliseconds"></param>
        /// <param name="intervalMilliseconds"></param>
        public static void GoToUrlWait(this IWebDriver webDriver, Uri url, int timeoutMilliseconds = 4000, int intervalMilliseconds = 500)
        {
            webDriver.Navigate().GoToUrl(url);
            webDriver.WaitForReadyState(timeoutMilliseconds, intervalMilliseconds);
        }

        /// <summary>
        /// Refreshes the current page.
        /// </summary>
        /// <param name="webDriver"></param>
        /// <param name="timeoutMilliseconds"></param>
        /// <param name="intervalMilliseconds"></param>
        public static void RefreshWait(this IWebDriver webDriver, int timeoutMilliseconds = 4000, int intervalMilliseconds = 500)
        {
            webDriver.Navigate().Refresh();
            webDriver.WaitForReadyState(timeoutMilliseconds, intervalMilliseconds);
        }
        #endregion

        #region Screenshot
        /// <summary>
        /// Saves the screenshot to a file, overwriting the file if it already exists.
        /// </summary>
        /// <param name="webDriver"></param>
        /// <param name="fileName"></param>
        public static void SaveScreenshot(this IWebDriver webDriver, string fileName, ScreenshotImageFormat format = ScreenshotImageFormat.Png)
        {
            ((ITakesScreenshot)webDriver).GetScreenshot().SaveAsFile(fileName, format);
        }

        /// <summary>
        /// Gets the value of the screenshot image as a Base64-encoded string.
        /// </summary>
        /// <param name="webDriver"></param>
        /// <returns></returns>
        public static string GetScreenshotBase64(this IWebDriver webDriver)
        {
            return ((ITakesScreenshot)webDriver).GetScreenshot().AsBase64EncodedString;
        }

        /// <summary>
        /// Gets the value of the screenshot image as a byte[].
        /// </summary>
        /// <param name="webDriver"></param>
        /// <returns></returns>
        public static byte[] GetScreenshotBytes(this IWebDriver webDriver)
        {
            return ((ITakesScreenshot)webDriver).GetScreenshot().AsByteArray;
        }

        public static void SaveScreenshotToFile(this IWebDriver webDriver, By path, string fileName, ImageFormat format)
        {
            IWebElement element = webDriver.FindElementWait(path, 5000, 3);
            IWebDriver wrappedDriver = ((IWrapsDriver)element).WrappedDriver;
            Rectangle rectangle = new Rectangle(element.Location.X, element.Location.Y, element.Size.Width, element.Size.Height);

            byte[] screenshot = wrappedDriver.GetScreenshotBytes();
            MemoryStream ms = new MemoryStream(screenshot);

            string directory = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            Bitmap image = new Bitmap(ms);
            Bitmap imageCropped = image.Clone(rectangle, image.PixelFormat);
            imageCropped.Save(fileName, format);
        }


        public static void SaveScreenshotAsJpeg(this IWebDriver webDriver, By path, string fileName)
        {
            SaveScreenshotToFile(webDriver, path, fileName, ImageFormat.Jpeg);
        }

        public static byte[] GetScreenshotFromElement(this IWebDriver webDriver, By path)
        {
            IWebElement element = webDriver.FindElementWait(path, 5000, 3);
            return GetScreenshotFromElement(webDriver, element);
        }

        public static byte[] GetScreenshotFromElement(this IWebDriver webDriver, IWebElement element)
        {
            byte[] screenshot = webDriver.GetScreenshotBytes();
            MemoryStream ms = new MemoryStream(screenshot);
            //TODO: Why using of SixLabors ?!
            Rectangle rectangle = new Rectangle(element.Location.X, element.Location.Y, element.Size.Width, element.Size.Height);

            Bitmap image = new Bitmap(ms);
            Bitmap imageCropped = image.Clone(rectangle, image.PixelFormat);
            return imageCropped.ImageToByteArray();

        }


        // extension method
        public static byte[] ImageToByteArray(this Image image)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, image.RawFormat);
            return ms.ToArray();
        }

        #endregion

        #region Actions
        public static Actions CreateActions(this IWebDriver webDriver)
        {
            return new Actions(webDriver);
        }

        /// <summary>
        /// Clicks the mouse at the last known mouse coordinates.
        /// </summary>
        /// <returns></returns>
        public static void Click(this IWebDriver webDriver)
        {
            webDriver.CreateActions().Click().Perform();
        }

        /// <summary>
        /// Clicks the mouse on the specified element.
        /// </summary>
        /// <param name="onElement"></param>
        /// <returns></returns>
        public static void Click(this IWebDriver webDriver, IWebElement onElement)
        {
            webDriver.CreateActions().Click(onElement).Perform();
        }

        /// <summary>
        /// Right-clicks the mouse at the last known mouse coordinates.
        /// </summary>
        /// <returns></returns>
        public static void ContextClick(this IWebDriver webDriver)
        {
            webDriver.CreateActions().ContextClick().Perform();
        }

        /// <summary>
        /// Right-clicks the mouse on the specified element.
        /// </summary>
        /// <param name="onElement"></param>
        /// <returns></returns>
        public static void ContextClick(this IWebDriver webDriver, IWebElement onElement)
        {
            webDriver.CreateActions().ContextClick(onElement).Perform();
        }

        /// <summary>
        /// Double-clicks the mouse at the last known mouse coordinates.
        /// </summary>
        /// <returns></returns>
        public static void DoubleClick(this IWebDriver webDriver)
        {
            webDriver.CreateActions().DoubleClick().Perform();
        }

        /// <summary>
        /// Double-clicks the mouse on the specified element.
        /// </summary>
        /// <param name="onElement"></param>
        /// <returns></returns>
        public static void DoubleClick(this IWebDriver webDriver, IWebElement onElement)
        {
            webDriver.CreateActions().DoubleClick(onElement).Perform();
        }

        /// <summary>
        /// Performs a drag-and-drop operation from one element to another.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static void DragAndDrop(this IWebDriver webDriver, IWebElement source, IWebElement target)
        {
            webDriver.CreateActions().DragAndDrop(source, target).Perform();
        }

        /// <summary>
        /// Performs a drag-and-drop operation on one element to a specified offset.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <returns></returns>
        public static void DragAndDropToOffset(this IWebDriver webDriver, IWebElement source, int offsetX, int offsetY)
        {
            webDriver.CreateActions().DragAndDropToOffset(source, offsetX, offsetY).Perform();
        }

        /// <summary>
        /// Sends a modifier key down message to the specified element in the browser.
        /// Exceptions : System.ArgumentException:
        /// If the key sent is not is not one of OpenQA.Selenium.Keys.Shift, OpenQA.Selenium.Keys.Control,
        /// OpenQA.Selenium.Keys.Alt, OpenQA.Selenium.Keys.Meta, OpenQA.Selenium.Keys.Command,OpenQA.Selenium.Keys.LeftAlt,
        /// OpenQA.Selenium.Keys.LeftControl,OpenQA.Selenium.Keys.LeftShift.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="theKey"></param>
        /// <returns></returns>
        public static void KeyDown(this IWebDriver webDriver, IWebElement element, string theKey)
        {
            webDriver.CreateActions().KeyDown(element, theKey).Perform();
        }

        /// <summary>
        /// Sends a modifier key down message to the browser.
        /// Exceptions : System.ArgumentException:
        /// If the key sent is not is not one of OpenQA.Selenium.Keys.Shift, OpenQA.Selenium.Keys.Control,
        /// OpenQA.Selenium.Keys.Alt, OpenQA.Selenium.Keys.Meta, OpenQA.Selenium.Keys.Command,OpenQA.Selenium.Keys.LeftAlt,
        /// OpenQA.Selenium.Keys.LeftControl,OpenQA.Selenium.Keys.LeftShift.
        /// </summary>
        /// <param name="theKey"></param>
        /// <returns></returns>
        public static void KeyDown(this IWebDriver webDriver, string theKey)
        {
            webDriver.CreateActions().KeyDown(theKey).Perform();
        }

        /// <summary>
        /// Sends a modifier up down message to the specified element in the browser.
        /// Exceptions : System.ArgumentException:
        /// If the key sent is not is not one of OpenQA.Selenium.Keys.Shift, OpenQA.Selenium.Keys.Control,
        /// OpenQA.Selenium.Keys.Alt, OpenQA.Selenium.Keys.Meta, OpenQA.Selenium.Keys.Command,OpenQA.Selenium.Keys.LeftAlt,
        /// OpenQA.Selenium.Keys.LeftControl,OpenQA.Selenium.Keys.LeftShift.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="theKey"></param>
        /// <returns></returns>
        public static void KeyUp(this IWebDriver webDriver, IWebElement element, string theKey)
        {
            webDriver.CreateActions().KeyUp(element, theKey).Perform();
        }

        /// <summary>
        /// Sends a modifier key up message to the browser.
        /// Exceptions : System.ArgumentException:
        /// If the key sent is not is not one of OpenQA.Selenium.Keys.Shift, OpenQA.Selenium.Keys.Control,
        /// OpenQA.Selenium.Keys.Alt, OpenQA.Selenium.Keys.Meta, OpenQA.Selenium.Keys.Command,OpenQA.Selenium.Keys.LeftAlt,
        /// OpenQA.Selenium.Keys.LeftControl,OpenQA.Selenium.Keys.LeftShift.
        /// </summary>
        /// <param name="theKey"></param>
        /// <returns></returns>
        public static void KeyUp(this IWebDriver webDriver, string theKey)
        {
            webDriver.CreateActions().KeyUp(theKey).Perform();
        }

        /// <summary>
        /// Moves the mouse to the specified offset of the last known mouse coordinates.
        /// </summary>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <returns></returns>
        public static void MoveByOffset(this IWebDriver webDriver, int offsetX, int offsetY)
        {
            webDriver.CreateActions().MoveByOffset(offsetX, offsetY).Perform();
        }

        /// <summary>
        /// Moves the mouse to the specified element.
        /// </summary>
        /// <param name="toElement"></param>
        /// <returns></returns>
        public static void MoveToElement(this IWebDriver webDriver, IWebElement toElement)
        {
            webDriver.CreateActions().MoveToElement(toElement).Perform();
        }

        /// <summary>
        /// Moves the mouse to the specified offset of the top-left corner of the specified element.
        /// </summary>
        /// <param name="toElement"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <returns></returns>
        public static void MoveToElement(this IWebDriver webDriver, IWebElement toElement, int offsetX, int offsetY)
        {
            webDriver.CreateActions().MoveToElement(toElement, offsetX, offsetY).Perform();
        }

        /// <summary>
        /// Moves the mouse to the specified offset of the top-left corner of the specified element.
        /// </summary>
        /// <param name="toElement"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="offsetOrigin"></param>
        /// <returns></returns>
        public static void MoveToElement(this IWebDriver webDriver, IWebElement toElement, int offsetX, int offsetY, MoveToElementOffsetOrigin offsetOrigin)
        {
            webDriver.CreateActions().MoveToElement(toElement, offsetX, offsetY, offsetOrigin).Perform();
        }

        /// <summary>
        /// Sends a sequence of keystrokes to the specified element in the browser.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="keysToSend"></param>
        /// <returns></returns>
        public static void SendKeys(this IWebDriver webDriver, IWebElement element, string keysToSend)
        {
            webDriver.CreateActions().SendKeys(element, keysToSend).Perform();
        }

        /// <summary>
        /// Sends a sequence of keystrokes to the browser.
        /// </summary>
        /// <param name="keysToSend"></param>
        /// <returns></returns>
        public static void SendKeys(this IWebDriver webDriver, string keysToSend)
        {
            webDriver.CreateActions().SendKeys(keysToSend).Perform();
        }
        #endregion

        #region Others
        public static void RunScriptWithWait(this IWebDriver webDriver, string function, int timeout, int maxTryCount)
        {
            int trycount = 0;
            while (trycount++ < maxTryCount)
            {
                try
                {
                    // new WebDriverWait(_webDriver, timeout, timeout).until(ExpectedConditions.javaScriptThrowsNoExceptions(function));
                    ((IJavaScriptExecutor)webDriver).ExecuteScript(function);
                    break;
                }
                catch (Exception e)
                {
                    Logger.LogError(e, $"Cannot execute script: {function}");
                }
                Thread.Sleep(timeout * 1000);
            }
        }

        public static void RunScriptWithWait(this IWebDriver webDriver, string function, int timeout)
        {
            RunScriptWithWait(webDriver, function, timeout, 4);
        }

        public static void CheckElementIsReady(this IWebDriver webDriver, By by, int timeout, int maxTryCount)
        {
            int trycount = 0;
            while (trycount++ < maxTryCount)
            {
                try
                {
                    WebDriverWait wait = new WebDriverWait(webDriver, new TimeSpan(0, 0, 30));
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(by));

                    //return new WebDriverWait(_webDriver, TimeSpan.FromSeconds(timeout))
                    //                        .Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(by))
                    //                        .FirstOrDefault();
                }
                catch (Exception e)
                {
                    Logger.LogError(e, $"Cannot CheckElementIsReady by: {by}");
                }
            }
            throw new Exception("Unable to find element in time");
        }

        public static void WaitForScript(this IWebDriver webDriver, string functionName, int timeout = 4, int retry = 2)
        {
            int tryCount = 0;
            string js = "return (typeof " + functionName + " !== 'undefined' && typeof " + functionName + " === 'function')";
            while (tryCount++ < retry)
            {
                try
                {
                    ((IJavaScriptExecutor)webDriver).ExecuteScript(js);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, $"Cannot execute script: {js}");
                    Thread.Sleep(timeout * 1000);
                }
            }
        }

        public static IWebDriver SwitchToFrame(this IWebDriver webDriver, IWebElement elemnt, int timeoutMilliseconds = 2000, int retryCount = 2)
        {
            return webDriver.DoOperation(driver => driver.SwitchTo().Frame(elemnt),
                timeoutMilliseconds, retryCount, ex => Logger.LogError(ex, "Cannot SwitchToFrame. Tag name: {tagName}", elemnt.TagName));
        }

        public static IWebDriver SwitchToFrame(this IWebDriver webDriver, string name, int timeoutMilliseconds = 2000, int retryCount = 2)
        {
            return webDriver.DoOperation(driver => driver.SwitchTo().Frame(name),
                 timeoutMilliseconds, retryCount, ex => Logger.LogError(ex, "Cannot SwitchToFrame. Frame name: {frameName}", name));
        }

        public static IWebDriver SwitchToFrame(this IWebDriver webDriver, int index, int timeoutMilliseconds = 2000, int retryCount = 2)
        {
            return webDriver.DoOperation(driver => driver.SwitchTo().Frame(index),
                 timeoutMilliseconds, retryCount, ex => Logger.LogError(ex, "Cannot SwitchToFrame. Frame index: {frameIndex}", index));
        }

        /// <summary>
        /// Instantiate a WebDriverWait that is used to check browser loading status on defined interval and timeout,
        /// Then run execute WebDriverWait's Until method with javascript that fetches "document.readyState" and matches
        /// to "complete" value.
        /// </summary>
        /// <param name="webDriver"></param>
        /// <param name="timeoutMilliseconds"></param>
        /// <param name="intervalMilliseconds"></param>
        /// <param name="exceptionTypes"></param>
        /// <returns></returns>
        public static bool WaitForReadyState(this IWebDriver webDriver, int timeoutMilliseconds = 60000, int intervalMilliseconds = 500, params Type[] exceptionTypes)
        {
            WebDriverWait webDriverWait = webDriver.GetWait(timeoutMilliseconds, intervalMilliseconds, exceptionTypes);
            return webDriverWait.Until(driver =>
            {
                driver.ExecuteScript("return document.readyState").Equals("complete");
                Thread.Sleep(200);
                return true;
            });
        }

        public static object ExecuteScript(this IWebDriver webDriver, string script, params object[] args)
        {
            if (!(webDriver is IJavaScriptExecutor))
            {
                throw new Exception("This driver does not support JavaScript!");
            }

            return ((IJavaScriptExecutor)webDriver).ExecuteScript(script, args);
        }

        // return element by executing javascript  
        public static IWebElement FindElementByExecuteScript(this IWebDriver webDriver, string script, int timeoutMilliseconds = 2000, int retryCount = 2)
        {
            if (!(webDriver is IJavaScriptExecutor))
            {
                throw new Exception("This driver does not support JavaScript!");
            }

            int tryCount = 0;
            while (tryCount++ < retryCount)
            {
                try
                {
                    // cast to IWebElement
                    return (IWebElement)((IJavaScriptExecutor)webDriver).ExecuteScript(script);
                }
                catch (WebDriverException)
                {
                    if (tryCount < retryCount)
                    {
                        Thread.Sleep(timeoutMilliseconds);
                    }
                    else
                    {
                        Logger.LogDebug($"Failed to find element by executing: {script} (attempt {tryCount}/{retryCount}");
                        throw new NotFoundException($"Cannot FindElement by {script}");
                    }

                }
            }
            throw new NotFoundException($"Cannot FindElement by {script}");
        }


        // return elements IWebElement collection by executing javascript  
        public static ReadOnlyCollection<IWebElement> FindElementsByExecuteScript(this IWebDriver webDriver, string script, int timeoutMilliseconds = 2000, int retryCount = 2)
        {
            if (!(webDriver is IJavaScriptExecutor))
            {
                throw new Exception("This driver does not support JavaScript!");
            }

            int tryCount = 0;
            while (tryCount++ < retryCount)
            {
                try
                {
                    // cast to IWebElement collection
                    return (ReadOnlyCollection<IWebElement>)((IJavaScriptExecutor)webDriver).ExecuteScript(script);
                }
                catch (WebDriverException)
                {
                    if (tryCount < retryCount)
                    {
                        Thread.Sleep(timeoutMilliseconds);
                    }
                    else
                    {
                        Logger.LogDebug($"Failed to find element by executing: {script} (attempt {tryCount}/{retryCount}");
                        throw new NotFoundException($"Cannot FindElement by {script}");
                    }

                }
            }
            throw new NotFoundException($"Cannot FindElement by {script}");
        }

        public static object ExecuteScriptAsync(this IWebDriver webDriver, string script, params object[] args)
        {
            if (!(webDriver is IJavaScriptExecutor))
            {
                throw new Exception("This driver does not support JavaScript!");
            }

            return ((IJavaScriptExecutor)webDriver).ExecuteAsyncScript(script, args);
        }

        public static bool WaitForFunction(this IWebDriver webDriver, string functionName, int timeoutMilliseconds = 2000, int retryCount = 2)
        {
            string script = "return (typeof " + functionName + " !== 'undefined' && typeof " + functionName + " === 'function')";

            return webDriver.DoOperation(driver =>
            {
                bool result = driver.ExecuteScript(script)?.Equals("true") == true;
                if (!result)
                {
                    throw null; //Throw Exception to retry 
                    }

                return result;
            }, timeoutMilliseconds, retryCount, ex => Logger.LogError(ex, "Cannot execute script: {script}", script));
        }

        public static object ExecuteScriptWait(this IWebDriver webDriver, string script, int timeoutMilliseconds = 2000, int retryCount = 2)
        {
            return webDriver.DoOperation(driver =>
                {
                    return driver.ExecuteScript(script);
                },
                timeoutMilliseconds, retryCount, ex => Logger.LogError(ex, "Cannot execute script: {script}", script));
        }

        /// <summary>
        /// XPath Helper function to find an element with given text using case insensitive approach.
        /// The only purpose to use this is to make each xpath line shorter in code
        /// </summary>
        /// <param name="webDriver"></param>
        /// <param name="input">Matching text</param>
        /// <returns></returns>
        public static string TextContains(this IWebDriver webDriver, string input)
        {
            return $"contains(translate(., 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'),'{input.ToLower()}')";
        }

        #endregion
    }
}
