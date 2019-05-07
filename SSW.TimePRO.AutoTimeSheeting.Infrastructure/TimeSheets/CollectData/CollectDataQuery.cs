using SSW.TimePRO.AutoTimeSheeting.Infrastructure.AzureDevOps;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.Crm;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.RecentProjects;
using System.Linq;
using System.Threading.Tasks;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.CollectData
{
    public class CollectDataQuery : ICollectDataQuery
    {
        private readonly IGetRecentProjectsQuery _getRecentProjectsQuery;
        private readonly IGetCrmAppointmentsQuery _getCrmAppointmentsQuery;
        private readonly IGetCommitsByEmpIDQuery _getCommitsByEmpIDQuery;

        public CollectDataQuery(
            IGetRecentProjectsQuery getRecentProjectsQuery,
            IGetCrmAppointmentsQuery getCrmAppointmentsQuery,
            IGetCommitsByEmpIDQuery getCommitsByEmpIDQuery)
        {
            _getRecentProjectsQuery = getRecentProjectsQuery;
            _getCrmAppointmentsQuery = getCrmAppointmentsQuery;
            _getCommitsByEmpIDQuery = getCommitsByEmpIDQuery;
        }

        public async Task<CollectDataModel> Execute(CollectDataRequest request)
        {
            var data = new CollectDataModel
            {
                EmpID = request.EmpID,
                Date = request.Date
            };

            var crmAppointmentsTask = _getCrmAppointmentsQuery.Execute(new GetCrmAppointmentsRequest(
                    request.TenantUrl,
                    request.EmpID,
                    request.Date,
                    request.Date,
                    request.Token));

            var recentProjectsTask = _getRecentProjectsQuery.Execute(new GetRecentProjectsRequest(
                    request.TenantUrl,
                    request.EmpID,
                    request.Token));

            data.CrmAppointments = await crmAppointmentsTask;

            if (data.CrmAppointments != null && data.CrmAppointments.Any(a => a.clientId != "SSW"))
            {
                data.Commits = await _getCommitsByEmpIDQuery.Execute(new GetCommitsByEmpIDRequest(
                    request.TenantUrl,
                    request.EmpID,
                    request.Date,
                    request.Token));
            }

            data.RecentProjects = await recentProjectsTask;

            return data;
        }
    }

    public interface ICollectDataQuery
    {
        Task<CollectDataModel> Execute(CollectDataRequest request);
    }
}
