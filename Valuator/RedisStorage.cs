using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Valuator
{
    public class RedisStorage : IStorage
    {
        private readonly ILogger<RedisStorage> _logger;

        public RedisStorage(ILogger<RedisStorage> logger) => this._logger = logger;

        public string Get(string key)
        {
            var db = this.GetDB();
            return db.StringGet(key);  
        }

        public List<string> GetAllText() {
            List<string> list = new List<string>();
            var connection = GetConnection();
            var server = connection.GetServer("localhost:6379");

           foreach(var key in server.Keys(pattern: "*TEXT-*")) {
                list.Add(Get(key));
            }

            return list;
        }

        public void Put(string key, string value)
        {
            var db = this.GetDB();
            db.StringSet(key, value);
        }

        private IDatabase GetDB() {
            ConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect("localhost");
            return connectionMultiplexer.GetDatabase();
        }

        private ConnectionMultiplexer GetConnection() {
            return ConnectionMultiplexer.Connect("localhost");
        }
    }

}