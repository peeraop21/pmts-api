using StackExchange.Redis;
using Newtonsoft.Json;
using PMTs.DataAccess.Redis.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace PMTs.DataAccess.Redis
{
    public class CacheService : ICacheService
    {
        private IDatabase _db;
        private IConfiguration _configuration;
        public CacheService(IConfiguration configuration)
        {
            _configuration = configuration;
            ConfigureRedis();
        }
        private void ConfigureRedis()
        {          
            try
            {
                var redisParameters = _configuration.GetSection("Redis").Get<RedisParameters>();
                if (redisParameters is null) throw new ArgumentNullException();
                var options = ConfigurationOptions.Parse(redisParameters.Server + ":" + redisParameters.Port.ToString());
                var connection = ConnectionMultiplexer.Connect(options);
                _db = connection.GetDatabase();
            }
            catch (StackExchange.Redis.RedisConnectionException)
            {
                _db = null;
            }
            catch (Exception)
            {
                _db = null;
            }

        }

        public T GetData<T>(string key)
        {
            if (_db is null) return default;
            var value = _db.StringGet(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            return default;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            if (_db is null) return default;
            TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            var isSet = _db.StringSet(key, JsonConvert.SerializeObject(value), expiryTime);
            return isSet;
        }

        public object RemoveData(string key)
        {
            if (_db is null) return default;
            bool _isKeyExist = _db.KeyExists(key);
            if (_isKeyExist == true)
            {
                return _db.KeyDelete(key);
            }
            return false;
        }
    }
}
