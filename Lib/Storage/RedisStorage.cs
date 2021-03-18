using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Lib
{
    public class RedisStorage : IStorage
    {
        private readonly string _host = "localhost";
        private readonly string _textSetKey = "textSetKey";
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
            return db.SetContains(_textSetKey, text);
        }

        public void Put(string key, string value)
        {
            var db = this.GetDB();
            db.StringSet(key, value);
        }

        public void PutTextToSet(string value)
        {
            var db = this.GetDB();
            db.SetAdd(_textSetKey, value);
        }
        
        private IDatabase GetDB() 
        {
            ConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(_host);
            return connectionMultiplexer.GetDatabase();
        }

        private ConnectionMultiplexer GetConnection() 
        {
            return ConnectionMultiplexer.Connect(_host);
        }
    }

}