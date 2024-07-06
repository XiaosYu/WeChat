namespace WeChat.WebApi.Servers.Config.Models
{
    public class ConnectionSetting
    {
        public string DB_WeChat { set; get; }
        public string this[string name] => this.GetType().GetProperty(name).GetValue(this) as string;
    }
}
