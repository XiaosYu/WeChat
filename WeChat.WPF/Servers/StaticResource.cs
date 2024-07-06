using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.WPF.Servers
{
    static public class StaticResource
    {
        static public string BaseUrl { get; set; }
        static public int BufferSize { get; set; }
        static public string WsUrl { get; set; }
        static public void Initialize()
        {
            var reader = new StreamReader("Settings.json");
            var token = JsonConvert.DeserializeObject<JObject>(reader.ReadToEnd());
            reader.Close();
            BaseUrl = (string)token["BaseUrl"];
            BufferSize = (int)token["BufferSize"];
            WsUrl = (string)token["WsUrl"];
        }
    }
}
