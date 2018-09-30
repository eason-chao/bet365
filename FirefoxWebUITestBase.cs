using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Threading;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Forms;

namespace bet365
{
    /// <summary>
    /// firefox 之Selenium UITest插件
    /// </summary>
    public class FirefoxWebUITest
    {
        static FirefoxWebUITest()
        {
            Initialize();
        }

        /// <summary>
        ///取得Web浏览器对象。
        /// </summary>
        public static IWebDriver Driver { get; set; }
        public static long timeOutInSeconds = 10;
        private static OpenQA.Selenium.Support.UI.SelectElement select = null;

        /// <summary>
        /// Driver初始化
        /// </summary>
        public static void Initialize()
        {
            //try
            //{
            //    var processes = Process.GetProcessesByName("firefox");
            //    if (processes.Length > 0)
            //    {
            //        foreach (var process in processes)
            //        {
            //            process.Kill();
            //        }
            //    }
            //}
            //catch
            //{

            //}
            //finally
            {
                FirefoxDriverService driverService = FirefoxDriverService.CreateDefaultService(@"C:\Program Files\Mozilla Firefox");
                driverService.FirefoxBinaryPath = @"C:\Program Files\Mozilla Firefox\firefox.exe";
                driverService.HideCommandPromptWindow = true;
                driverService.SuppressInitialDiagnosticInformation = true;
                Driver = new FirefoxDriver(driverService);
            }
        }


        /**
        * 打开URL
        * @param url
        * @param browser
        */
        public static void OpenBrowser(string url)
        {
            try
            {
                var s = Driver.Url;
            }
            catch (Exception)
            {
                Initialize();
            }
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeOutInSeconds);
            Driver.Manage().Window.Maximize();
            Driver.Navigate().GoToUrl(url);
        }

        /**
        * 关闭所有selenium驱动打开的浏览器
        */
        public static void CloseAll()
        {
            Thread.Sleep(3000);
            Driver.Quit();
            Driver.Dispose();
        }


        /**
         * 刷新页面
        */
        public static void Refresh()
        {
            Driver.Navigate().Refresh();
        }


        /// <summary>
        /// 弹出对话框点击确定
        /// </summary>
        /// <returns></returns>
        public static string AcceptAlert()
        {
            try
            {
                IAlert alert = Driver.SwitchTo().Alert();
                string alertText = alert.Text;
                alert.Accept();
                return alertText;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 关闭或取消弹出对话框
        /// </summary>
        /// <returns></returns>
        public static string DismissAlert()
        {
            try
            {
                IAlert alert = Driver.SwitchTo().Alert();
                string alertText = alert.Text;
                alert.Dismiss();
                return alertText;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /**
        * 指定时间内等待直到页面包含文本字符串
        * @param text    期望出现的文本
        * @param seconds    超时时间
        * @return    Boolean    检查给定文本是否存在于指定元素中, 超时则捕获抛出异常TimeoutException并返回false
        * @see    org.openqa.selenium.support.ui.ExpectedConditions.textToBePresentInElement(WebElement element, String text)
        */
        public static Boolean WaitUntilPageContainText(String text, TimeSpan seconds)
        {
            try
            {
                return new WebDriverWait(Driver, seconds).Until(ExpectedConditions.TextToBePresentInElement
                        (Driver.FindElement(By.TagName("body")), text));
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /**
        * 默认时间等待直到页面包含文本字符串
        * @param text    期望出现的文本
        * @param seconds    超时时间
        * @return    Boolean    检查给定文本是否存在于指定元素中, 超时则捕获抛出异常TimeoutException并返回false
        * @see    org.openqa.selenium.support.ui.ExpectedConditions.textToBePresentInElement(WebElement element, String text)
        */
        public static Boolean WaitUntilPageContainText(String text)
        {
            try
            {
                return new WebDriverWait(Driver, TimeSpan.FromSeconds(timeOutInSeconds)).Until(ExpectedConditions.TextToBePresentInElement
                        (Driver.FindElement(By.TagName("body")), text));
            }
            catch (Exception e)
            {
                return false;
            }
        }


        /**
       * 默认时间等待直到页面包含该控件
       * @param text    期望出现的文本
       * @param seconds    超时时间
       * @return    Boolean    检查给定文本是否存在于指定元素中, 超时则捕获抛出异常TimeoutException并返回false
       * @see    org.openqa.selenium.support.ui.ExpectedConditions.textToBePresentInElement(WebElement element, String text)
       */
        public static IWebElement WaitUntilPageExistsControl(By by)
        {
            try
            {
                IWebElement control = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeOutInSeconds)).Until(ExpectedConditions.ElementExists(by));
                Thread.Sleep(2000);
                return control;
            }
            catch (Exception e)
            {
                return null;
            }
        }



        /**
         * 下拉选择框根据选项文本值选择, 即当text="Bar", 那么这一项将会被选择:
         * &lt;option value="foo"&gt;Bar&lt;/option&gt;
         * @param locator
         * @param text
         * @see org.openqa.selenium.support.ui.Select.selectByVisibleText(String text)
         */
        public static void SelectByText(By locator, String text)
        {
            select = new SelectElement(Driver.FindElement(locator));
            select.SelectByText(text);
        }

        /**
         * 下拉选择框根据索引值选择, 这是通过检查一个元素的“index”属性来完成的, 而不仅仅是通过计数
         * @param locator
         * @param index
         * @see    org.openqa.selenium.support.ui.Select.selectByIndex(int index)
         */
        public static void SelectByIndex(By locator, int index)
        {
            select = new SelectElement(Driver.FindElement(locator));
            select.SelectByIndex(index);
        }

        /**
         * 下列选择框根据元素属性值(value)选择, 即value = "foo" , 那么这一项将会被选择:
         * &lt;option value="foo"&gt;Bar&lt;/option&gt;
         * @param locator
         * @param value
         * @see    org.openqa.selenium.support.ui.Select.selectByValue(String value)
         */
        public static void SelectByValue(By locator, String value)
        {
            select = new SelectElement(Driver.FindElement(locator));
            select.SelectByValue(value);
        }

        /**
        * 切换frame
        * @param locator
        * @return    这个驱动程序切换到给定的frame
        * @see     org.openqa.selenium.Driver.TargetLocator.frame(WebElement frameElement)
        */
        public static void SwitchToFrame(By locator)
        {
            Driver.SwitchTo().Frame(Driver.FindElement(locator));
        }

        /**
         * 切换回父frame
         * @return    这个驱动程序聚焦在顶部窗口/第一个frame上
         * @see    org.openqa.selenium.Driver.TargetLocator.defaultContent()
         */
        public static void SwitchToParentFrame()
        {
            Driver.SwitchTo().DefaultContent();
        }


        /**
     * 根据cookie名称删除cookie
     * @param name    cookie的name值
     * @see    org.openqa.selenium.Driver.Options.deleteCookieNamed(String name)
     */
        public static void DeleteCookie(String name)
        {
            Driver.Manage().Cookies.DeleteCookieNamed(name);
        }


        /**
     * 删除当前域的所有Cookie
     * @see    org.openqa.selenium.Driver.Options.deleteAllCookies()
     */
        public static void DeleteAllCookies()
        {
            Driver.Manage().Cookies.DeleteAllCookies();
        }

        /**
        * 根据名称获取指定cookie
        * @param name    cookie名称
        * @return    Map&lt;String, String>, 如果没有cookie则返回空, 返回的Map的key值如下:<ul>
        *             <li><tt>name</tt>        <tt>cookie名称</tt>
        *             <li><tt>value</tt>        <tt>cookie值</tt>
        *             <li><tt>path</tt>        <tt>cookie路径</tt>
        *             <li><tt>domain</tt>        <tt>cookie域</tt>
        *             <li><tt>expiry</tt>        <tt>cookie有效期</tt>
        *             </ul>
        * @see    org.openqa.selenium.Driver.Options.getCookieNamed(String name)
        */
        public static Dictionary<String, String> GetCookieByName(String name)
        {
            Cookie cookie = Driver.Manage().Cookies.GetCookieNamed(name);
            if (cookie != null)
            {
                Dictionary<String, String> map = new Dictionary<String, String>();
                map.Add("name", cookie.Name);
                map.Add("value", cookie.Value);
                map.Add("path", cookie.Path);
                map.Add("domain", cookie.Domain);
                map.Add("expiry", cookie.Expiry.HasValue ? cookie.Expiry.Value.ToString() : "");
                return map;
            }
            return null;
        }


        /// <summary>
        /// 截取控件图片。
        /// </summary>
        /// <param name="control">控件。</param>
        public static void CapturePicture(string fileName)
        {
            try
            {
                Screenshot ss = ((ITakesScreenshot)Driver).GetScreenshot();
                ss.SaveAsFile(fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss")+".jpg", ScreenshotImageFormat.Jpeg);//use any of the built in image formating
            }
            catch
            {
            }
        }

         

        /**
         * 获取当前域所有的cookies
         * @return    Set&lt;Cookie>    当前的cookies集合
         * @see    org.openqa.selenium.Driver.Options.getCookies()
         */
        public static ReadOnlyCollection<Cookie> GetAllCookies()
        {
            return Driver.Manage().Cookies.AllCookies;
        }

        /**
         * 用给定的name和value创建默认路径的Cookie并添加, 永久有效
         * @param name
         * @param value
         * @see    org.openqa.selenium.Driver.Options.addCookie(Cookie cookie)
         * @see org.openqa.selenium.Cookie.Cookie(String name, String value)
         */
        public static void AddCookie(String name, String value)
        {
            Driver.Manage().Cookies.AddCookie(new Cookie(name, value));
        }

        /**
         * 用给定的name和value创建指定路径的Cookie并添加, 永久有效
         * @param name    cookie名称
         * @param value    cookie值
         * @param path    cookie路径
         */
        public static void AddCookie(String name, String value, String path)
        {
            Driver.Manage().Cookies.AddCookie(new Cookie(name, value, path));
        }
    

        /// <summary>
        /// 根据条件查询控件是否存在
        /// </summary>
        /// <param name="by"></param>
        /// <returns></returns>
        public static bool IsElementPresent(By by)
        {
            try
            {
                Driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// alert弹出框是否存在
        /// </summary>
        /// <returns></returns>
        public bool IsAlertPresent()
        {
            try
            {
                Driver.SwitchTo().Alert();                
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }

    }
}
