
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using NATS.Client;
using System.Text;
using Valuator;
using System.Text.Json;
using Lib;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IStorage _storage;
        private readonly IPublisher _publisher;

        public IndexModel(ILogger<IndexModel> logger, IStorage storage, IPublisher publisher)
        {
            _logger = logger;
            _storage = storage;
            _publisher = publisher;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost(string text, string shardId)
        {
            if (string.IsNullOrEmpty(text)) 
            {
                return Redirect("/");
            }

            string id = Guid.NewGuid().ToString();
            string textKey = Constants.TextKeyPrefix + id;
            string rankKey = Constants.RankKeyPrefix + id;  
            string similarityKey = Constants.SimilarityKeyPrefix + id;

            _logger.LogInformation($"LOOKUP: {id} {shardId}");

            _storage.PutShardId(id, shardId);
            _storage.Put(shardId, textKey, text);
            _storage.PutTextToSet(shardId, text);
            _storage.Put(shardId, similarityKey, GetSimilarity(text).ToString());

            SendLoggerInfo(id, GetSimilarity(text).ToString());

            CalculateAndStoreRank(id);
            
            return Redirect($"summary?id={id}");
        }

        private void CalculateAndStoreRank(string id)
        {
            _publisher.Publish(Constants.CalculateRankSubject, id);
        }

        private void SendLoggerInfo(string context, string value) 
        {
            SimilarityInfo info = new SimilarityInfo(context, value);
            _publisher.Publish(Constants.SimilarityCalculatedEvent, GetSerializedInfo(info));
        }

        private int GetSimilarity(string text) 
        {
            return _storage.HasTextDuplicate(text) ? 1 : 0;
        }

        static string GetSerializedInfo(SimilarityInfo info)
        {
            return JsonSerializer.Serialize(info);
        }
    }
}
