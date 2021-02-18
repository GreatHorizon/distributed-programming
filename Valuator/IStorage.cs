using System.Collections.Generic;
using StackExchange.Redis;

namespace Valuator
{
    public interface IStorage 
    {
        void Put(string key, string value);
        string Get(string key);

        public bool HasTextDuplicate(string text);
    }
}