using System;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.RecentProjects.Queries
{
    public class RecentProjectModel
    {
        public string Client { get; set; }
        public string ClientID { get; set; }
        public string Project { get; set; }
        public string ProjectID { get; set; }
        public string Iteration { get; set; }
        public string IterationId { get; set; }
        public string Category { get; set; }
        public string CategoryID { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
