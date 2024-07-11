using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1.Services
{
    public interface IDataConverter
    {
        public Task<string> GetDataTypeMessageAsync(object value);

    }
}
