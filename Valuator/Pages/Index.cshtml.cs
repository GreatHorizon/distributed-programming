
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

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IStorage _storage;

        public IndexModel(ILogger<IndexModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost(string text)
        {

            if (string.IsNullOrEmpty(text)) 
            {
                return Redirect("/");
            }

            string id = Guid.NewGuid().ToString();
            string textKey = Constants.TextKeyPrefix + id;
            string rankKey = Constants.RankKeyPrefix + id;  
            string similarityKey = Constants.SimilarityKeyPrefix + id;

            int similarity = GetSimilarity(text);
            _storage.Put(textKey, text);
            _storage.PutTextToSet(text);
            _storage.Put(similarityKey, similarity.ToString());
            CalculateAndStoreRank(id);
            
            return Redirect($"summary?id={id}");
        }

        private void CalculateAndStoreRank(string id)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Task.Factory.StartNew(() =>ProduceAsync(id, cts.Token), cts.Token);
        }

        private int GetSimilarity(string text) 
        {
            return _storage.HasTextDuplicate(text) ? 1 : 0;
        }

        static async Task ProduceAsync(string id, CancellationToken ct)
        {
            ConnectionFactory cf = new ConnectionFactory();

            using (IConnection c = cf.CreateConnection())
            {
                byte[] data = Encoding.UTF8.GetBytes(id);
                c.Publish("valuator.processing.rank", data);
                c.Drain();
                c.Close();
            }
        }
    }
}
