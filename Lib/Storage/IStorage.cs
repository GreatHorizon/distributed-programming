using System.Collections.Generic;
using StackExchange.Redis;

namespace Lib 
{
    public interface IStorage 
    {
        void Put(string shardKey, string key, string value);
        string Get(string shardKey, string key);
        void PutTextToSet(string shardKey, string value);

        bool HasTextDuplicate(string text);

        void PutShardId(string key, string shardId);

        string GetShardId(string key);

    }
}