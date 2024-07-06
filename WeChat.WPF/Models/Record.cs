using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.WPF.Models
{
    public class Record
    {
        public string Sender { set; get; }
        public string Receiever { set; get; }
        public DateTime DateTime { set; get; }
        public string Message { set; get; }
    }
}
