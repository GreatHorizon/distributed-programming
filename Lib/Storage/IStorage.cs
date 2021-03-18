using System.Collections.Generic;
using StackExchange.Redis;

namespace Lib 
{
    public interface IStorage 
    {
        void Put(string key, string value);
        string Get(string key);
        void PutTextToSet(string value);

        public bool HasTextDuplicate(string text);
    }
}