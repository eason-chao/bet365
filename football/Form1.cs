using bet365;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace football
{
    public partial class Form1 : Form
    {

        public static Dictionary<string, string> dic = new Dictionary<string, string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            FirefoxWebUITest.OpenBrowser("https://mobile.48-365365.com/default.aspx?apptype=&appversion=&rnd=84776#type=InPlay;key=;ip=1;lng=2");
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

        private void money()
        {

            try
            {
                //var control = FirefoxWebUITest.WaitUntilPageExistsControl(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='投注單'])[1]/following::div[7]"));
                //if (control != null)
                //{
                //    control.Click();
                //}
                while (true)
                {
                    catchControlValue(getNames());
                    Thread.Sleep(10000);
                }
            }
            catch (Exception ex)
            {

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


        public void catchControlValue(string[] names)
        {
            InvokeControlAction(textBox1, new Action<TextBox>((b) =>
            {
                b.Text += "\r\n" + "------------------------华丽分割线,必出BUG-----------------------------" + "\r\n";
            }));
            int i = -1;
            foreach (var item in names)
            {
                i++;
                if (i % 2 != 0)
                {
                    continue;
                }
                try
                {
                    InvokeControlAction(textBox2, new Action<TextBox>((b) =>
                    {
                        b.Text = "\r\n正在采集  " + item + "\r\n";
                    }));


                    var viewBtn = FirefoxWebUITest.Driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='" + item + "'])[1]/following::div[13]"));
                    if (string.IsNullOrEmpty(viewBtn.Text))
                    {
                        viewBtn.Click();
                    }
                    else
                    {
                        if (FirefoxWebUITest.Driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='" + item + "'])[1]/following::div[14]")).Text == "" && FirefoxWebUITest.Driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='" + item + "'])[1]/following::div[11]")).Text != "")
                            FirefoxWebUITest.Driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='" + item + "'])[1]/following::div[14]")).Click();
                    }

                    var sourceA = FirefoxWebUITest.Driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='" + item + "'])[1]/following::div[11]")).Text;
                    var sourceB = FirefoxWebUITest.Driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='" + item + "'])[1]/following::div[12]")).Text;
                    var time = FirefoxWebUITest.Driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='" + item + "'])[1]/following::div[6]")).Text;
                    if (int.Parse(time.Split(':')[0]) > 30)
                    {
                        var con1 = FirefoxWebUITest.Driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='射正球門'])[1]/following::b[1]")).Text;
                        var con2 = FirefoxWebUITest.Driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='射正球門'])[1]/following::b[2]")).Text;

                        var con333 = FirefoxWebUITest.Driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='危險進攻'])[1]/following::div[1]")).Text;
                        var con3 = con333.Split(Environment.NewLine.ToCharArray())[0];
                        var con4 = con333.Split(Environment.NewLine.ToCharArray())[2];

                        //+分盘或 平局 
                        bool a = sourceA.Split(Environment.NewLine.ToCharArray())[0] == "0.0" && !(sourceA.Split(Environment.NewLine.ToCharArray())[0].Contains(","));
                        bool b = sourceB.Split(Environment.NewLine.ToCharArray())[0] == "0.0" && !(sourceB.Split(Environment.NewLine.ToCharArray())[0].Contains(","));

                        if ((a || sourceA.Contains("+")) && float.Parse(sourceA.Split(Environment.NewLine.ToCharArray())[2]) >= 1.7)
                        {
                            if (int.Parse(con1) > int.Parse(con2) && int.Parse(con3) > int.Parse(con4))
                            {//射正球門
                                var score1 = FirefoxWebUITest.Driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='" + item + "'])[1]/following::div[1]")).Text;
                                var score2 = FirefoxWebUITest.Driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='" + item + "'])[1]/following::div[5]")).Text;
                                if (int.Parse(score1) > int.Parse(score2))
                                {
                                    continue;
                                }
                                if (!dic.ContainsKey(item + "1"))
                                {
                                    dic.Add(item + "1", "1");

                                    InvokeControlAction(textBox1, new Action<TextBox>((txt) =>
                                    {
                                        txt.Text += item + " 买1" + "\r\n";
                                    }));

                                    Thread vThread = new Thread(new Send().playSystemMic);
                                    vThread.Start(); //开始执行线程
                                }
                            }
                        }
                        //+分盘或 平局 
                        if ((b || sourceB.Contains("+")) && float.Parse(sourceB.Split(Environment.NewLine.ToCharArray())[2]) >= 1.7)
                        {
                            if (int.Parse(con2) > int.Parse(con1) && int.Parse(con4) > int.Parse(con3))
                            {//射正球門

                                var score1 = FirefoxWebUITest.Driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='" + item + "'])[1]/following::div[1]")).Text;
                                var score2 = FirefoxWebUITest.Driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='" + item + "'])[1]/following::div[5]")).Text;
                                if (int.Parse(score2) > int.Parse(score1))
                                {
                                    continue;
                                }
                                if (!dic.ContainsKey(item + "2"))
                                {
                                    dic.Add(item + "2", "2");
                                    InvokeControlAction(textBox1, new Action<TextBox>((txt) =>
                                    {
                                        txt.Text += item + " 买2" + "\r\n";
                                    }));

                                    Thread vThread = new Thread(new Send().playSystemMic);
                                    vThread.Start(); //开始执行线程
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }

                Thread.Sleep(1000);
            }
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

        private void button2_Click(object sender, EventArgs e)
        {
           
            System.Action handler = new System.Action(this.money);     //定义委托
            handler.BeginInvoke(null, null);           //异步调用
        }
    }
}
