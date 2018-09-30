using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenQA.Selenium;
using System.Threading;
using System.Net.Mail;
using Qcloud.Sms;
using System.Runtime.InteropServices;
using System.Media;

namespace bet365
{
//曾经的梦想
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        static Dictionary<string, int> result = new Dictionary<string, int>();
        static bool isBegin = true;

        public void catchControlValue(string[] controls)
        {
            textBox1.Text = string.Empty;
            foreach (var item in controls)
            {
                try
                {
                    //var value1 = FirefoxWebUITest.Driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='" + item + "'])[1]/following::span[1]"));
                    var value = FirefoxWebUITest.Driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='" + item + "'])[1]/following::span[2]"));
                    InvokeControlAction(textBox1, new Action<TextBox>((b) =>
                    {
                        b.Text += item + "：" + value.Text + "\r\n";
                    }));
                    var key = item;
                    if (result.ContainsKey(key))
                    {
                        int source = Int32.Parse(value.Text);
                        if (result[key] - source == 1 && source != 0)
                        {
                            InvokeControlAction(txtResult, new Action<TextBox>((b) =>
                            {
                                b.Text = "恭喜发财：" + item + " ：now：" + value.Text + ",old：" + result[key].ToString() + "\r\n";
                            }));

                            Thread vThread = new Thread(new Send().playSystemMic);
                            vThread.Start(); //开始执行线程

                            //邮件
                            Send s = new Send();
                            s.name = item;
                            s.now = value.Text;
                            s.old = result[key].ToString();
                            //Thread vThread2 = new Thread(s.sendMail);
                            //vThread2.Start(); //开始执行线程

                            //Thread vThread3 = new Thread(s.sendMsm);
                            //vThread3.Start(); //开始执行线程

                            Thread vThread4 = new Thread(s.sendMail2);
                            vThread4.Start(); //开始执行线程

                            //NewMethod(item, value);
                        }
                        result[key] = source;
                    }
                    else
                    {
                        result.Add(key, Int32.Parse(value.Text));
                    }
                }
                catch (Exception ex)
                {
                    FirefoxWebUITest.Driver.Navigate().GoToUrl(textBox3.Text);
                }
            }
        }

        private void NewMethod(string item, IWebElement value)
        {
            try
            {
                value.Click();
                //if (FirefoxWebUITest.IsElementPresent(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='點擊以切換至其它比賽'])[1]/following::div[1]")))
                //{
                //    FirefoxWebUITest.Driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='點擊以切換至其它比賽'])[1]/following::div[1]")).Click();
                //}
                string html = FirefoxWebUITest.Driver.PageSource;
                int iBodyStart = html.IndexOf("局獲勝者", 0);    //开始位置
                if (iBodyStart != -1)
                {
                    int iBodyEnd = html.IndexOf("</span>", iBodyStart);   //第二次字符在第一次字符位置起的首次位置
                    if (iBodyEnd != -1)
                    {
                        int indeA = iBodyStart - 2;
                        int indeB = iBodyEnd - iBodyStart + 2;
                        string currentSituation = html.Substring(indeA, indeB);//当前对局
                        if (!currentSituation.Contains("第"))
                        {
                            currentSituation = "第" + currentSituation;
                        }
                        string serveName = item.Trim() + " (發球局)";//發球局人的名字
                        string matchName = Crawler(html, "<span class=\"ipe-EventViewTitle_Text\">", "</span>");
                        if (!string.IsNullOrEmpty(matchName))
                        {
                            string div = Crawler(html, currentSituation, "<div class=\"ipe-Market_FavStar \"></div>");
                            string a = matchName.Substring(0, matchName.IndexOf(" v  ")).Trim();
                            string b = matchName.Substring(matchName.IndexOf(" v  ") + 3).Trim();
                            if (div.Contains(serveName))//发球者是item
                            {
                                FirefoxWebUITest.Driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='" + currentSituation + "'])[1]/following::div[3]")).Click();
                            }
                            else
                            {
                                if (item != a)
                                {
                                    serveName = a + " (發球局)";
                                    FirefoxWebUITest.Driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='" + serveName + "'])[1]/following::div[1]")).Click();
                                }
                                if (item != b)
                                {
                                    serveName = b + " (發球局)";
                                    FirefoxWebUITest.Driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='" + serveName + "'])[1]/following::div[1]")).Click();
                                }
                            }
                            matchName = a + " v " + b;
                            FirefoxWebUITest.Driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='" + matchName + "'])[1]/following::input[1]")).Click();
                            FirefoxWebUITest.Driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='" + matchName + "'])[1]/following::input[1]")).Clear();
                            FirefoxWebUITest.Driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='" + matchName + "'])[1]/following::input[1]")).SendKeys("2");
                            if (FirefoxWebUITest.IsElementPresent(By.ClassName("acceptChanges abetslipBtn")))
                            {
                                FirefoxWebUITest.Driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='走地盤投注會稍有延遲'])[1]/following::button[1]")).Click();//赔率变更
                            }
                            FirefoxWebUITest.Driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='接受变化'])[1]/following::button[1]")).Click();//投钱
                            if (FirefoxWebUITest.IsElementPresent(By.LinkText("保留這些選項")))
                            {
                                FirefoxWebUITest.Driver.FindElement(By.LinkText("保留這些選項")).Click();
                            }
                            Thread.Sleep(60000);
                            FirefoxWebUITest.Driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='投注單'])[1]/following::a[1]")).Click();//后退按钮
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FirefoxWebUITest.Refresh();
                FirefoxWebUITest.WaitUntilPageExistsControl(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='投注單'])[1]/following::a[1]")).Click();//后退按钮
            }
        }

        public string[] getNames()
        {
            int iBodyEnd = 0;
            List<string> name = new List<string>();
            var html = FirefoxWebUITest.Driver.PageSource;
            while (iBodyEnd >= 0)
            {
                var str = Crawler(html, "<div class=\"ipo-Fixture_Truncator \">", "</div>", ref iBodyEnd);
                if (!string.IsNullOrEmpty(str))
                {
                    name.Add(str.Replace("</div", "").Trim());
                }
            }
            return name.ToArray();
        }

        private string Crawler(string istr, string startString, string endString)
        {
            int iBodyEnd = 0;
            //初始化out参数,否则不能return
            int iBodyStart = istr.IndexOf(startString, iBodyEnd);    //开始位置
            if (iBodyStart == -1)
            {
                iBodyEnd = -1;
                return null;
            }
            iBodyStart += startString.Length;       //第一次字符位置起的长度
            iBodyEnd = istr.IndexOf(endString, iBodyStart);   //第二次字符在第一次字符位置起的首次位置
            if (iBodyEnd == -1)
                return null;
            //iBodyEnd += endString.Length;        //第二次字符位置起的长度
            string strResult = istr.Substring(iBodyStart, iBodyEnd - iBodyStart);
            return strResult;
        }


        private string Crawler(string istr, string startString, string endString, ref int iBodyEnd)
        {
            //初始化out参数,否则不能return
            int iBodyStart = istr.IndexOf(startString, iBodyEnd);    //开始位置
            if (iBodyStart == -1)
            {
                iBodyEnd = -1;
                return null;
            }
            iBodyStart += startString.Length;       //第一次字符位置起的长度
            iBodyEnd = istr.IndexOf(endString, iBodyStart);   //第二次字符在第一次字符位置起的首次位置
            if (iBodyEnd == -1)
                return null;
            iBodyEnd += endString.Length;        //第二次字符位置起的长度
            string strResult = istr.Substring(iBodyStart, iBodyEnd - iBodyStart - 1);
            return strResult;
        }

        public void findKey(string key)
        {
            Thread.Sleep(5000);
            var control = FirefoxWebUITest.WaitUntilPageExistsControl(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='" + key + "'])[2]/following::div[1]"));
            if (control != null)
            {
                control.Click();
            }
            else
            {
                InvokeControlAction(textBox1, new Action<TextBox>((b) =>
                {
                    b.Text = "没找到" + key + "关键字，继续查找" + "\r\n";
                }));
                findKey(key);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            //邮件
            System.Action handler = new System.Action(this.money);     //定义委托
            handler.BeginInvoke(null, null);           //异步调用
        }

        public static uint SND_ASYNC = 0x0001;
        public static uint SND_FILENAME = 0x00020000;
        [DllImport("winmm.dll")]
        public static extern uint mciSendString(string lpstrCommand,
        string lpstrReturnString, uint uReturnLength, uint hWndCallback);
        public void Play()
        {
            string path = "\"" + System.Environment.CurrentDirectory + "\\Fade.mp3\"";
            mciSendString(@"close temp_alias", null, 0, 0);
            mciSendString(@"open " + path + " alias temp_alias", null, 0, 0);
            mciSendString("play temp_alias repeat", null, 0, 0);
        }

       

        public static bool isF = false;
        private void money()
        {
            try
            {
                isF = !isF;
                var productKey = textBox2.Text;
                FirefoxWebUITest.OpenBrowser(textBox3.Text);
                findKey(productKey);
                while (true)
                {
                    catchControlValue(getNames());
                    //catchControlValue(new string[] { "A.科奈特", "伊萬•宋" });
                    InvokeControlAction(textBox1, new Action<TextBox>((b) =>
                    {
                        b.Text += b.Text + "\r\n" + "------------------------华丽分割线,必出BUG-----------------------------" + "\r\n";
                    }));
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                if (isF)
                {
                    textBox3.Text = "https://mobile.48-365365.com/default.aspx?apptype=&appversion=&rnd=66392#type=InPlay;key=;ip=1;lng=2";
                }
                else
                {
                    textBox3.Text = "https://mobile.356884.com/#type=InPlay;key=;ip=1;lng=28";
                }
                InvokeControlAction(textBox1, new Action<TextBox>((b) =>
                {
                    b.Text = "StackTrace:" + ex.StackTrace + "Message:" + ex.Message + "\r\n";
                }));
                FirefoxWebUITest.CloseAll();
                Thread.Sleep(3000);
                money();
            }
        }

        public void pause()
        {
            if (btnPause.Text == "Pause")
            {
                isBegin = false;
                btnPause.Text = "Begin";
            }
            else
            {
                btnPause.Text = "Pause";
                isBegin = true;
            }
        }

        /// <summary>
        /// 通用控件异步显示信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="control"></param>
        /// <param name="action"></param>
        public static void InvokeControlAction<T>(T control, Action<T> action) where T : Control
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new Action<T, Action<T>>(InvokeControlAction),
                    new object[] { control, action });
            }
            else
            {
                action(control);
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            System.Action handler = new System.Action(this.pause);     //定义委托
            handler.BeginInvoke(null, null);           //异步调用
        }
    }
}
