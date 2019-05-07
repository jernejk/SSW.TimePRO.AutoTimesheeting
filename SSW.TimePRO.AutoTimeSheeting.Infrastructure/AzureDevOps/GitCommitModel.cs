using System;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.AzureDevOps
{
    public class GitCommitResult
    {
        public object[] errors { get; set; }
        public GitCommitModel[] data { get; set; }
    }

    public class GitCommitModel
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Comment { get; set; }
        public string By { get; set; }
        public DateTime Date { get; set; }
        public int Changes { get; set; }
        public string TfsConnectionName { get; set; }
        public string FormattedDate { get; set; }
    }

}
