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
        
        public RankCalculator(IStorage storage, ILogger<RankCalculator> logger)
        {
            _storage = storage;
            _logger = logger;
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

        private void EmitRankCalculatedEvent(string info)
        {
            using (var conn = new ConnectionFactory().CreateConnection())
            {
                conn.Publish(Constants.RankCalculatedEvent, Encoding.UTF8.GetBytes(info));
            }
        }

        private string GetSerializedInfo(RankInfo info)
        {
            return JsonSerializer.Serialize(info);
        }

        private void SendLoggerInfo(string context, string value)
        {
            var info = new RankInfo(context, value);
            CancellationTokenSource cts = new CancellationTokenSource();
            Task.Factory.StartNew(() => EmitRankCalculatedEvent(GetSerializedInfo(info)), cts.Token);
        }

        private IAsyncSubscription CreateSubscription(IConnection connection)
        {
            return connection.SubscribeAsync(Constants.SubjectName, Constants.QueueGroupName, (sender, args) =>
            {
                string key = Encoding.UTF8.GetString(args.Message.Data);
                string text = _storage.Get(Constants.TextKeyPrefix + key);
                string rank = CalculateRank(text).ToString();

                _storage.Put(Constants.RankKeyPrefix + key, rank);
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
    }
    
}