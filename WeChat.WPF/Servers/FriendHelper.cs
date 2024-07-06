using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.WPF.Servers
{
    internal class FriendHelper
    {
        public FriendHelper(string uid)
        {
            BaseUrl = StaticResource.BaseUrl;
            Uid = uid;
            Client = new();
        }
        private string BaseUrl = "";
        private string Uid = "";
        private HttpClient Client = null;

        public async Task<Models.Group> ListFriends()
        {
            HttpRequestMessage message = new HttpRequestMessage();
            message.Method = HttpMethod.Post;
            message.RequestUri = new Uri($"{BaseUrl}/api/friend/listfriends?uid={Uid}");
            var result = await Client.SendAsync(message);
            if(result.StatusCode!=HttpStatusCode.OK)
            {
                return null;
            }
            else
            {
                var data = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Models.Group>(data.Replace(" ",""));
            }
        }

        public async Task<Models.Group> SearchFriend(string key)
        {
            HttpRequestMessage message = new HttpRequestMessage();
            message.Method = HttpMethod.Post;
            message.RequestUri = new Uri($"{BaseUrl}/api/friend/searchfriend?uid={Uid}&key={key}");
            var result = await Client.SendAsync(message);
            if (result.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
            else
            {
                var data = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Models.Group>(data);
            }
        }

        public async Task<Models.Group> SearchUser(string key)
        {
            HttpRequestMessage message = new HttpRequestMessage();
            message.Method = HttpMethod.Post;
            message.RequestUri = new Uri($"{BaseUrl}/api/friend/searchuser?uid={Uid}&key={key}");
            var result = await Client.SendAsync(message);
            if (result.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
            else
            {
                var data = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Models.Group>(data);
            }
        }

        public async Task<bool> AddRelation(string point)
        {
            HttpRequestMessage message = new HttpRequestMessage();
            message.Method = HttpMethod.Post;
            message.RequestUri = new Uri($"{BaseUrl}/api/friend/addrelation?uid={Uid}&point={point}");
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

        public async Task<bool> DeleteRelation(string point)
        {
            HttpRequestMessage message = new HttpRequestMessage();
            message.Method = HttpMethod.Post;
            message.RequestUri = new Uri($"{BaseUrl}/api/friend/deleterelation?uid={Uid}&point={point}");
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

        public async Task<bool> CheckRelation(string point)
        {
            HttpRequestMessage message = new HttpRequestMessage();
            message.Method = HttpMethod.Post;
            message.RequestUri = new Uri($"{BaseUrl}/api/friend/checkrelation?uid={Uid}&point={point}");
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
    }
}
