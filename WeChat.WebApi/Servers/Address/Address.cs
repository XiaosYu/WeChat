namespace WeChat.WebApi.Servers.Address
{
    public class Address: ExternWebApiBase
    {
        public string GetAddress(string ip)
        {
            string url = "http://whois.pconline.com.cn/ip.jsp";
            string data = RequestByGet(url,
                $"ip={ip}",
                $"level=3"
                );
            return data;
        }
    }
}
