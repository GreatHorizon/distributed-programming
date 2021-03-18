using System;
using NATS.Client;
using System.Text.Json;
using Lib;

namespace EventLogger
{
    class EventLogger
    {
        private IConnection _connection;
        
        private EventHandler<MsgHandlerEventArgs> _rankCalculatedCallback = (sender, args) =>
        {
            RankInfo info = DeserializeRankInfo(args.Message.Data);

            Console.WriteLine($"Event: {args.Message.Subject}");
            Console.WriteLine($"Context: {info.context}");
            Console.WriteLine($"Rank: {info.rankValue}");
            Console.WriteLine();

        };

        private EventHandler<MsgHandlerEventArgs> _similarityCalculatedCallback = (sender, args) =>
        {
            SimilarityInfo info = DeserializeSimilarityInfo(args.Message.Data);

            Console.WriteLine($"Event: {args.Message.Subject}");
            Console.WriteLine($"Context: {info.context}");
            Console.WriteLine($"Similarity: {info.similarityValue}");
            Console.WriteLine();

        };
            
        private static RankInfo DeserializeRankInfo(byte[] jsonUtf8Bytes)
        {
            var readOnlySpan = new ReadOnlySpan<byte>(jsonUtf8Bytes);
            return JsonSerializer.Deserialize<RankInfo>(readOnlySpan);
        }

        private static SimilarityInfo DeserializeSimilarityInfo(byte[] jsonUtf8Bytes)
        {
            var readOnlySpan = new ReadOnlySpan<byte>(jsonUtf8Bytes);
            return JsonSerializer.Deserialize<SimilarityInfo>(readOnlySpan);
        }
        
        public EventLogger()   
        {
            _connection = CreateConnection();

            _connection.SubscribeAsync(Constants.RankCalculatedEvent, _rankCalculatedCallback);
            _connection.SubscribeAsync(Constants.SimilarityCalculatedEvent, _similarityCalculatedCallback);
        }

        ~EventLogger()
        {
            _connection.Drain();
        }

        private IConnection  CreateConnection()
        {
            IConnection c = new ConnectionFactory().CreateConnection();
            return c;
        }
    }
}