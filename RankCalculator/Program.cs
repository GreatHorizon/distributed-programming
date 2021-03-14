using Valuator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace RankCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var storage = new RedisStorage(new Logger<RedisStorage>(loggerFactory));
            var calculator = new RankCalculator(storage, loggerFactory.CreateLogger<RankCalculator>());
            calculator.Start();
        }
    }
}
