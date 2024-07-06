using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.WPF.Models
{
    public class Bucket
    {
        public Bucket()
        {
        }

        private List<Record> Data = new List<Record>();
        public void Put(Record data) => Data.Add(data);
        public void Delete(Record data) => Data.Remove(data);
        public void ForEach(Action<Record> action) => Data.ForEach(action);
        public List<Record> GetUserMessage(string uid,string point)
            => point
            == "HOST" ?
            Data.Where(s => s.Sender == "HOST").ToList():
            Data.Where(s => (s.Receiever == uid && s.Sender == point) || (s.Sender == uid && s.Receiever == point)).OrderBy(s => s.DateTime).ToList();

       

        public void Clear(string uid,string point)
        {
            var result = GetUserMessage(uid, point);
            lock(this)
            {
                for(int i=0;i<Data.Count;i++)
                {
                    Delete(result[i]);
                }                      
            }
        }
        
    }
}
