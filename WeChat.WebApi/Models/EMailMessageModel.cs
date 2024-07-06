namespace WeChat.WebApi.Models
{
    public class EMailMessageModel
    {
        public string Head { set; get; }
        public string Body { set; get; }
        public List<string> Address { set; get; }
    }
}
