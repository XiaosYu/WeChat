namespace WeChat.WPF.Models.Chat
{
    public class BaseModel<T> where T:IBody
    {
        public string Verfiy { set; get; }
        public string Guid { set; get; }
        public string Action { set; get; }
        public string Sender { set; get; }
        public string Receiever { set; get; }
        public T Body { set; get; }
    }
}
