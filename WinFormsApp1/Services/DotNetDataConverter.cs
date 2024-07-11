using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1.Services
{
    public class DotNetDataConverter : IDataConverter
    {
        public async Task<string> GetDataTypeMessageAsync(object value)
        {
            await Task.Delay(1000);
            return value.ToString() ?? "";
        }
    }
}
