using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Lib
{
    public class RedisStorage : IStorage
    {
        private readonly string _host = "localhost";
        private readonly string _textSetKey = "textSetKey";

        private IConnectionMultiplexer _connection;
        private Dictionary<string, IConnectionMultiplexer> _shardsConnections;

        public RedisStorage(ILogger<RedisStorage> logger) 
        {
            _connection = ConnectionMultiplexer.Connect(_host);
            _shardsConnections = CreateShardsConnectons();
        }


        private Dictionary<string, IConnectionMultiplexer> CreateShardsConnectons()
        {
            var connections = new Dictionary<string, IConnectionMultiplexer>();

            connections.Add(Constants.RusShardId, 
            ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("DB_RUS", EnvironmentVariableTarget.User)));
            
            connections.Add(Constants.EuShardId, 
            ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("DB_EU", EnvironmentVariableTarget.User)));
            
            connections.Add(Constants.OtherShardId, 
            ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("DB_OTHER", EnvironmentVariableTarget.User)));
            
            return connections;    
        }

        public string Get(string shardKey, string key)
        {
            var connection = GetShardConnection(shardKey);
            var db = _connection.GetDatabase();
            return db.StringGet(key);  
        }

        public bool HasTextDuplicate(string text) 
        {
            var rusConnection = GetShardConnection(Constants.RusShardId);
            var euConnection = GetShardConnection(Constants.EuShardId);
            var otherConnection = GetShardConnection(Constants.OtherShardId);

            return rusConnection.GetDatabase().SetContains(_textSetKey, text) 
                || euConnection.GetDatabase().SetContains(_textSetKey, text)
                || otherConnection.GetDatabase().SetContains(_textSetKey, text);
        }

        public void Put(string shardKey, string key, string value)
        {
            var connection = GetShardConnection(shardKey);
            var db = _connection.GetDatabase();
            db.StringSet(key, value);
        }

        public void PutTextToSet(string shardKey, string value)
        {
            var connection = GetShardConnection(shardKey);
            var db = connection.GetDatabase();
            db.SetAdd(_textSetKey, value);
        }

        private IConnectionMultiplexer GetShardConnection(string shardKey) 
        {
            return _shardsConnections[shardKey];
        }

        public string GetShardId(string key)
        {
            var db = _connection.GetDatabase();
            return db.StringGet(key);
        }

        public void PutShardId(string key, string shardId)
        {
            var db = _connection.GetDatabase();
            db.StringSet(key, shardId);
        }
    }

}