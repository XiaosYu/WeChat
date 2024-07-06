

namespace WeChat.WPF.Servers
{
    internal class UserHelper
    {
        public UserHelper()
        {
            BaseUrl = StaticResource.BaseUrl;
            Client = new();
        }
        private string BaseUrl = "";
        private HttpClient Client = null;

        public async Task<User> Login(string uid,string pwd)
        {
            HttpRequestMessage message = new();
            message.RequestUri = new System.Uri($"{BaseUrl}/api/user/login?uid={uid}&pwd={pwd}");
            message.Method = HttpMethod.Post;
            var result = await Client.SendAsync(message);
            if (result.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
            else
            {
                return JsonConvert.DeserializeObject<User>(await result.Content.ReadAsStringAsync());
            }
        }
        public async Task<bool> Register(string phone,string pwd,string name)
        {
            HttpRequestMessage message = new();
            message.RequestUri = new System.Uri($"{BaseUrl}/api/user/register?phone={phone}&pwd={pwd}&name={name}");
            message.Method = HttpMethod.Post;
            var result = await Client.SendAsync(message);
            if (result.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public async Task<bool> Reset(string phone,string newpwd)
        {
            HttpRequestMessage message = new();
            message.RequestUri = new System.Uri($"{BaseUrl}/api/user/reset?phone={phone}&newpwd={newpwd}");
            message.Method = HttpMethod.Post;
            var result = await Client.SendAsync(message);
            if (result.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public async Task<User> Verfiy(string phone,string code)
        {
            HttpRequestMessage message = new();
            message.RequestUri = new System.Uri($"{BaseUrl}/api/user/verfiy?phone={phone}&code={code}");
            message.Method = HttpMethod.Post;
            var result = await Client.SendAsync(message);
            if (result.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
            else
            {
                JObject token = JsonConvert.DeserializeObject<JObject>(await result.Content.ReadAsStringAsync());
                if(token.ContainsKey("msg"))
                {
                    //说明是重置密码
                    return new User();
                }
                else
                {
                    var user = token.ToObject<User>();
                    user.EMail = "";
                    user.Sign = "";
                    return user;
                }
            }
        }
        public async Task<bool> Modify(string uid,string name="",string sign="")
        {
            HttpRequestMessage message = new();
            message.Method = HttpMethod.Post;
            message.RequestUri = new System.Uri($"{BaseUrl}/api/user/modify?uid={uid}&name={name}&sign={sign}");
            var result = await Client.SendAsync(message);
            if(result.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<UNameCollection> GetUserNames(string uid)
        {
            HttpRequestMessage message = new();
            message.Method = HttpMethod.Post;
            message.RequestUri = new System.Uri($"{BaseUrl}/api/user/getusernames?uid={uid}");
            var result = await Client.SendAsync(message);
            if(result.StatusCode!= HttpStatusCode.OK)
            {
                return null;
            }
            else
            {
                return JsonConvert.DeserializeObject<UNameCollection>(await result.Content.ReadAsStringAsync());
            }
        }
    }
}
