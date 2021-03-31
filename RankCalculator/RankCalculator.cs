using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NATS.Client;
using System.Text.Json;
using Lib;

namespace RankCalculator
{
    class RankCalculator
    {
        private IStorage _storage;
        private ILogger<RankCalculator> _logger;
        private IPublisher _publisher;
        
        public RankCalculator(IStorage storage, ILogger<RankCalculator> logger, IPublisher publisher)
        {
            _storage = storage;
            _logger = logger;
            _publisher = publisher;
        }

        public void Start()
        {
            IConnection connection = CreateConnection();
            IAsyncSubscription subscription = CreateSubscription(connection);

            subscription.Start();

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();

            subscription.Unsubscribe();
            connection.Drain();
        }
        
        private IConnection CreateConnection()
        {
            ConnectionFactory cf = new ConnectionFactory();
            return cf.CreateConnection();
        }

        private void SendLoggerInfo(string context, string value)
        {
            var info = new RankInfo(context, value);
            _publisher.Publish(Constants.RankCalculatedEvent, GetSerializedInfo(info));
        }

        private IAsyncSubscription CreateSubscription(IConnection connection)
        {
            return connection.SubscribeAsync(Constants.CalculateRankSubject, Constants.CalculateRankQueueGroupName, (sender, args) =>
            {
                string key = Encoding.UTF8.GetString(args.Message.Data);
                _logger.LogInformation($"LOOKUP: {key} {_storage.GetShardId(key)}");
                string text = _storage.Get(_storage.GetShardId(key), Constants.TextKeyPrefix + key);
                string rank = CalculateRank(text).ToString();

                _storage.Put(_storage.GetShardId(key), Constants.RankKeyPrefix + key, rank);
                SendLoggerInfo(key, rank);
            });
        }

        private double CalculateRank(string text)
        {
            int nonAlphaCount = 0;
            foreach (var symbol in text)
            {
                if (!Char.IsLetter(symbol)) 
                {
                    nonAlphaCount++;
                }
            }
            
            double rank = Math.Round(Convert.ToDouble(nonAlphaCount) / Convert.ToDouble(text.Length), 2);
            
            string logText = $"{text.Substring(0, Math.Min(10, text.Length))} Length: {text.Length} Non alpha count: {nonAlphaCount} Rank {rank}";
           _logger.LogInformation(logText);

            return rank;
        }


        static string GetSerializedInfo(RankInfo info)
        {
            return JsonSerializer.Serialize(info);
        }

    }
    
}