namespace Lib
{
    public static class Constants
    {
        public const string TextKeyPrefix = "TEXT-";
        public const string RankKeyPrefix = "RANK-";
        public const string SimilarityKeyPrefix = "SIMILARITY-";
        public const string CalculateRankSubject = "valuator.processing.rank";
        public const string CalculateRankQueueGroupName = "rank_calculator";

        public const string RankCalculatedEvent = "RankCalculated";
        public const string SimilarityCalculatedEvent = "SimilarityCalculated";

        public const string RusShardId = "RUS";
        public const string EuShardId = "EU";
        public const string OtherShardId = "OTHER";
    }
} 