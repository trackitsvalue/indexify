namespace indexify.Models
{
    public class ReindexResultModel
    {
        public string IndexSource { get; set; }
        public string IndexDestination { get; set; }
        public int DocsInOldIndex { get; set; }
        public int DocsCreated { get; set; }
        public int DocsUpdated { get; set; }
        public int DocsVersionConflicts { get; set; }
        public int DocsDeleted { get; set; }
        public long ActionTime { get; set; }
        public bool DeleteAcknowledged { get; set; }
        public bool ReindexSkipped { get; set; }
        public bool DeleteIndexSkipped { get; set; }
        public string SkippedReason { get; set; }
        public bool ExceptionEncountered { get; set; }
        public string ExceptionInfo { get; set; }
    }
}
