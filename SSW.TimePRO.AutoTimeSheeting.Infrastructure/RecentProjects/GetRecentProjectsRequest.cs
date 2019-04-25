namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.RecentProjects
{
    public class GetRecentProjectsRequest : BaseTimeProRequest
    {
        public GetRecentProjectsRequest(string tenantUrl, string empID, string token)
            : base(tenantUrl, token)
        {
            EmpID = empID;
        }

        public string EmpID { get; }
    }
}
