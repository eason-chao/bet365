using System.Collections.Generic;
using System.Net.Mail;
using System.Threading;

namespace bet365
{
    public class Send
    {

        public string name;
        public string now;
        public string old;

        public int playTime = 0;
        public void playSystemMic()
        {
            while (playTime < 8)
            {
                playTime++;
                System.Media.SystemSounds.Asterisk.Play();
                System.Media.SystemSounds.Beep.Play();
                System.Media.SystemSounds.Exclamation.Play();
                System.Media.SystemSounds.Hand.Play();
                System.Media.SystemSounds.Question.Play();
                Thread.Sleep(300);
            }
        }


        public void sendMail()
        {
            try
            {
                var strContent = string.Format("恭喜发财[{3}]：{0}：now：{1}，old：{2}", name, now, old, System.DateTime.Now.ToString());
                var strSubject = "恭喜发财";//标题
                var strFrom = "18549921992@163.com";
                var strFromPass = "s18549921992";
                var strSmtpServer = "smtp.163.com";//Smtp服务器
                var client = new SmtpClient(strSmtpServer);
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(strFrom, strFromPass);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();//(strFrom, strTo, strSubject, strBody);
                message.From = new MailAddress(strFrom, strSubject);
                message.To.Add("626797421@qq.com");
                //message.To.Add("31308436@qq.com");
                //message.To.Add("243491137@qq.com");
                message.Subject = strSubject;
                message.Body = strContent;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.IsBodyHtml = true;
                client.SendAsync(message, sendOk());
            }
            catch { }
        }


        public object sendOk()
        {
            return "ok";
        }
    }
}
