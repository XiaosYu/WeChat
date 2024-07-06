namespace WeChat.WebApi.Models
{
    public class VerifyCodeModel<T>
    {
        public string Phone { set; get; }
        public string Code { set; get; }
        public DateTime RegisterTime { set; get; }
        public Func<T> CallBack { set; get; }
    }
}
