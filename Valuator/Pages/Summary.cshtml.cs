using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading;
using Lib;

namespace Valuator.Pages
{
    public class SummaryModel : PageModel
    {
        private readonly ILogger<SummaryModel> _logger;
        private readonly IStorage _storage;
        public SummaryModel(ILogger<SummaryModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public double Rank { get; set; }
        public double Similarity { get; set; }

        public void OnGet(string id)
        {
            string rank = _storage.Get(_storage.GetShardId(id), Constants.RankKeyPrefix + id);

            _logger.LogInformation($"LOOKUP: {id} {_storage.GetShardId(id)}");

            for (int i = 0; i < 20; i++)
            {
                if (!string.IsNullOrEmpty(rank))
                {
                    break;
                }
                Thread.Sleep(200);
                rank = _storage.Get(_storage.GetShardId(id), Constants.RankKeyPrefix + id);
            }

            Rank = Convert.ToDouble(rank);
            Similarity = Convert.ToDouble(_storage.Get(_storage.GetShardId(id), Constants.SimilarityKeyPrefix + id));
        }
    }
}
