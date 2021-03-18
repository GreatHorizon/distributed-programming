namespace Lib
{
    public class SimilarityInfo
    {
        public string context { get; set; }
        public string similarityValue { get; set; }

        public SimilarityInfo(string id, string similarity)
        {
            context = id;
            similarityValue = similarity;
        }

        public SimilarityInfo() {}
    }
}