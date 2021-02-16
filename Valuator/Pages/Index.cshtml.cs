using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

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

            if (string.IsNullOrEmpty(text)) {
                return Redirect("/");
            }

            string id = Guid.NewGuid().ToString();
            string textKey = "TEXT-" + id;
            string rankKey = "RANK-" + id;  
            string similarityKey = "SIMILARITY-" + id;

            int similarity = GetSimilarity(text);
            double rank = GetRank(text);
            _logger.LogWarning(rank.ToString());

            _storage.Put(textKey, text);
            _storage.Put(similarityKey, similarity.ToString());
            _storage.Put(rankKey, rank.ToString());
            
            return Redirect($"summary?id={id}");
        }

        private double GetRank(string text) {
            int nonAlphaCount = 0;
            foreach (var symbol in text)
            {
                if (!Char.IsLetter(symbol)) 
                {
                    nonAlphaCount++;
                }
            }

            return Math.Round(Convert.ToDouble(nonAlphaCount) / Convert.ToDouble(text.Length), 2);
        }
        private int GetSimilarity(string text) {
            List<string> texts = _storage.GetAllText();
            
            foreach (string str in texts) {
                if (str == text) 
                {
                    return 1; 
                }
            }

            return 0;
        }
    }
}
