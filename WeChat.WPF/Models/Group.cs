using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.WPF.Models
{
    public class Group
    {
        public int Code { set; get; }
        public int Count { set; get; }

        public List<People> Data { set; get; }
    }

    public class People
    {
        public string Quid { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Sign { get; set; }
        public override string ToString()
        {
            return this.Name;
        }
    }
}
