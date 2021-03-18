namespace Lib
{
    public class RankInfo
    {
        public string context { get; set; }
        public string rankValue { get; set; }
        
        public RankInfo(string id, string rank) 
        {
            context = id;
            rankValue = rank;
        }

        public RankInfo() {}
    }
}