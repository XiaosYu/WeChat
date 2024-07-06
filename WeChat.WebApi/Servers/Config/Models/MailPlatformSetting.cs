namespace WeChat.WebApi.Servers.Config.Models
{
    public class MailPlatformSetting
    {
        public List<EMailSetting> Platform { get; set; }
        public int Count => Platform.Count;
    }
    public class EMailSetting
    {
        public string EMail { get; set; }
        public string Key { get; set; }
    }
}
