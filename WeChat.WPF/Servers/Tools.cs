using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.WPF.Servers
{
    static public class Tools
    {
        static public string Random(int length, string dic = "")
        {
            dic = dic == "" ? "qwertyuiopasdfghjklzxcvbnm0123456789QWERTYUIOPASDFGHJKLZXCVBNM" : dic == "-1" ? "0123456789" : dic;
            string data = ""; Random rand = new Random();
            for (int i = 0; i < length; i++)
            {
                data += dic[rand.Next(dic.Length)];
            }
            return data;
        }
    }
}
