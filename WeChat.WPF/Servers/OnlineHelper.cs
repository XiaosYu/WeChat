using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.WPF.Servers
{
    public class OnlineHelper
    {
        public OnlineHelper(string uid)
        {
            BaseUrl = StaticResource.BaseUrl;
            Uid = uid;
            Client = new();
        }
        private string BaseUrl = "";
        private string Uid = "";
        private HttpClient Client = null;

        public async Task<bool> IsOnline(string point)
        {
            HttpRequestMessage message = new HttpRequestMessage();
            message.RequestUri = new Uri($"{BaseUrl}/api/online/isonline?uid={Uid}&point={point}");
            message.Method = HttpMethod.Post;
            var result = await Client.SendAsync(message);
            if(result.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<Models.Group> OnlineUser()
        {
            HttpRequestMessage message = new HttpRequestMessage();
            message.RequestUri = new Uri($"{BaseUrl}/api/online/onlineuser?uid={Uid}");
            message.Method = HttpMethod.Post;
            var result = await Client.SendAsync(message);
            if(result.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
            else
            {
                var data = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Models.Group>(data);
            }
        }
    }
}
