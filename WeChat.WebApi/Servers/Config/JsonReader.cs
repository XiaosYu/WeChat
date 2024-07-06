namespace WeChat.WebApi.Servers.Config
{
    public class Config
    {
        public T GetSetting<T>(string path)where T:class
        {
            StreamReader reader = new(path);
            var data = reader.ReadToEnd();
            reader.Close();
            return JsonConvert.DeserializeObject<T>(data);
        }
       
    }
}
