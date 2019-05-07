using System;
using System.Collections.Generic;
using System.Text;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.AzureDevOps
{

    public class AzureDevOpsSubscriptionResult
    {
        public object[] errors { get; set; }
        public AzureDevOpsSubscription[] data { get; set; }
    }

    public class AzureDevOpsSubscription
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public object EmpId { get; set; }
    }
}
