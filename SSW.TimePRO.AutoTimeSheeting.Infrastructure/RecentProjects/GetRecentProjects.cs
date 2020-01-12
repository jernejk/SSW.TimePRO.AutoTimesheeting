using MediatR;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.RecentProjects
{
    public class GetRecentProjects : BaseTimeProRequest, IRequest<RecentProjectModel[]>
    {
        public GetRecentProjects(string tenantUrl, string empID, string token)
            : base(tenantUrl, token)
        {
            EmpID = empID;
        }

        public string EmpID { get; }
    }
}
