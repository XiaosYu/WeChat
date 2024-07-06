

namespace WeChat.WebApi.Servers.Mail
{
    public class MailBuilder
    {
        public MailBuilder() { }
        public List<string>? Address { set; get; } = new List<string>();
        public string? Body { set; get; } = string.Empty;
        public Encoding? Encoding { set; get; } = Encoding.UTF8;
        public bool? IsHtml { set; get; } = true;
        public string? Subject { set; get; } = string.Empty;
        public List<string>? Attachments { set; get; } = new List<string>();
        public MailMessage Build()
        {
            var message = new MailMessage();
            if (Address?.Count == 0) throw new Exception("发送人不能为空");
            foreach (var c in Address)
            {
                message.To.Add(c);
            }
            message.Subject = Subject;
            message.Body = Body;
            message.IsBodyHtml = IsHtml ?? true;
            message.BodyEncoding = Encoding;
            message.Priority = MailPriority.Normal;
            message.SubjectEncoding = Encoding;
            foreach(var c in Attachments)
            {
                if (!File.Exists(c)) throw new Exception("附件文件不存在");
                var data = new Attachment(c, MediaTypeNames.Application.Octet);//实例化附件 
                data.ContentDisposition.CreationDate = File.GetCreationTime(c);
                data.ContentDisposition.ModificationDate = File.GetLastAccessTime(c);
                data.ContentDisposition.ReadDate = DateTime.Now;
                message.Attachments.Add(data);//添加到附件中 
            }
            return message;
        }
        public Task<MailMessage> BuildAsync()
        {
            Task<MailMessage> message = new Task<MailMessage>(() =>
            {
                var message = new MailMessage();
                if (Address?.Count == 0) throw new Exception("发送人不能为空");
                foreach (var c in Address)
                {
                    message.To.Add(c);
                }
                message.Subject = Subject;
                message.Body = Body;
                message.IsBodyHtml = IsHtml ?? true;
                message.BodyEncoding = Encoding;
                message.Priority = MailPriority.Normal;
                message.SubjectEncoding = Encoding;
                foreach (var c in Attachments)
                {
                    if (!File.Exists(c)) throw new Exception("附件文件不存在");
                    var data = new Attachment(c, MediaTypeNames.Application.Octet);//实例化附件 
                    data.ContentDisposition.CreationDate = File.GetCreationTime(c);
                    data.ContentDisposition.ModificationDate = File.GetLastAccessTime(c);
                    data.ContentDisposition.ReadDate = DateTime.Now;
                    message.Attachments.Add(data);//添加到附件中 
                }
                return message;
            });
            message.Start();
            return message;
        }
        
    }
}
