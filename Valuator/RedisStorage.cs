using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Valuator
{
    public class RedisStorage : IStorage
    {
        private readonly string host = "localhost";
        private readonly string TextSetKey = "textSetKey";
        private readonly ILogger<RedisStorage> _logger;

        public RedisStorage(ILogger<RedisStorage> logger) => this._logger = logger;

        public string Get(string key)
        {
            var db = this.GetDB();
            return db.StringGet(key);  
        }

        public bool HasTextDuplicate(string text) 
        {
            var db = GetDB();
            return db.SetContains(TextSetKey, text);
        }

        public void Put(string key, string value)
        {
            var db = this.GetDB();
            if (key.StartsWith(Constants.TextKeyPrefix))
            {
                db.SetAdd(TextSetKey, value);
            }
            db.StringSet(key, value);
        }

        private IDatabase GetDB() 
        {
            ConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(Host);
            return connectionMultiplexer.GetDatabase();
        }

        private ConnectionMultiplexer GetConnection() 
        {
            return ConnectionMultiplexer.Connect(Host);
        }
    }

}