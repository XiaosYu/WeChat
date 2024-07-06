namespace WeChat.WebApi.Servers.Mail
{
    public class MailPlatform
    {
        private string _last;
        public string LastAllocatedMail { get => _last; }
        private List<(MailAddress,string)>? _mailinformation;
        public MailPlatform(params (MailAddress,string)[]? pairs)
        {
            _mailinformation = pairs.ToList() ?? throw new Exception("错误的初始化");
        }
        public MailPlatform(params EMailSetting[]? pairs)
        {
            _mailinformation = new List<(MailAddress, string)>();
            foreach(var c in pairs)
            {
                (MailAddress, string) data = (new MailAddress(c.EMail), c.Key);
                _mailinformation.Add(data);
            }
            if (_mailinformation.Count == 0) throw new Exception("错误的初始化");

        }
        public void SendMail(MailMessage message)
        {
            var random = new Random();
            var info = _mailinformation?.OrderBy(s => Guid.NewGuid()).FirstOrDefault() ?? throw new Exception("未初始化任何设置");
            _last = info.Item1.Address;
            message.From = info.Item1;
            var client = new SmtpClient()
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(info.Item1.Address, info.Item2),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = "smtp." + info.Item1.Host
            };
            client.Send(message);
        }
        public void SendMailAsync(MailMessage message, SendCompletedEventHandler CompletedMethod, object args)
        {
            var random = new Random();
            var info = _mailinformation?.OrderBy(s => Guid.NewGuid()).FirstOrDefault() ?? throw new Exception("未初始化任何设置");
            _last = info.Item1.Address;
            message.From = info.Item1;
            var client = new SmtpClient()
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(info.Item1.Address, info.Item2),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = "smtp." + info.Item1.Host
            };
            client.SendCompleted += new SendCompletedEventHandler(CompletedMethod);
            client.SendAsync(message, args);
        }
        public Task SendMailAsync(MailMessage message)
        {
            var random = new Random();
            var info = _mailinformation?.OrderBy(s => Guid.NewGuid()).FirstOrDefault() ?? throw new Exception("未初始化任何设置");
            _last = info.Item1.Address;
            Task task = new Task(() =>
              {               
                  message.From = info.Item1;
                  var client = new SmtpClient()
                  {
                      EnableSsl = true,
                      UseDefaultCredentials = false,
                      Credentials = new System.Net.NetworkCredential(info.Item1.Address, info.Item2),
                      DeliveryMethod = SmtpDeliveryMethod.Network,
                      Host = "smtp." + info.Item1.Host
                  };
                  client.Send(message);
              });
            task.Start();
            return task;        
        }
    }
}
