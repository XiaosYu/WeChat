using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.WPF.Models
{
    public class UNameCollection
    {
        public List<UName> Data { get; set; }

        public string SearchName(string uid,Func<UNameCollection> CallBack)
        {
            var result = Data.Where(s => s.Uid == uid).FirstOrDefault();
            if(result == null)
            {
                //未找到
                Data = CallBack().Data;
                var data = Data.Where(s => s.Uid == uid).FirstOrDefault();
                if (data == null) return "暂无此名称";
                return data.Name.TrimEnd();
            }
            else
            {
                return result.Name.TrimEnd();
            }
        }
    }

    public class UName
    {
        public string Uid { set; get; }
        public string Name { set; get; }
    }


}
