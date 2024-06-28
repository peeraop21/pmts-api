using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMTs.DataAccess.Redis.Interfaces
{
    public interface ICacheService
    {
        T GetData<T>(string key);
        object RemoveData(string key);
        bool SetData<T>(string key, T value, DateTimeOffset expirationTime);
    }
}
