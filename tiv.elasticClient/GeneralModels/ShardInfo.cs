namespace tiv.elasticClient.GeneralModels
{
    public class ShardInfo
    {
        public int total { get; set; }
        public int successful { get; set; }
        public int skipped { get; set; }
        public int failed { get; set; }
    }
}
