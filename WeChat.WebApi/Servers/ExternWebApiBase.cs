namespace WeChat.WebApi.Servers
{
    public class ExternWebApiBase
    {
        protected string RequestByGet(string baseurl,params string[] args)
        {
            string url = baseurl + "?";
            url += string.Join('&', args);            
            HttpClient client = new HttpClient();           
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage response = client.Send(request);
            return response.Content.ReadAsStringAsync().Result;            
        }
        protected T RequestByGet<T>(string baseurl, params string[] args)
        {
            string url = baseurl + "?";
            url += string.Join('&', args);
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage response = client.Send(request);
            return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
        }
        protected string RequestByPost(string baseurl, object args)
        {
            string url = baseurl;
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            //拼凑json
            JsonContent content = JsonContent.Create(args);
            request.Content = content;
            HttpResponseMessage response = client.Send(request);
            return response.Content.ReadAsStringAsync().Result;
        }
        protected T RequestByPost<T>(string baseurl, object args)
        {
            string url = baseurl;
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            //拼凑json
            JsonContent content = JsonContent.Create(args);
            request.Content = content;
            HttpResponseMessage response = client.Send(request);
            return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
        }
    }
}
