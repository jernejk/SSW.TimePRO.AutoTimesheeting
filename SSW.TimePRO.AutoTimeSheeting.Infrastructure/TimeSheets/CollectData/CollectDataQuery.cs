using SSW.TimePRO.AutoTimeSheeting.Infrastructure.Crm;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.RecentProjects;
using System.Threading.Tasks;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.CollectData
{
    public class CollectDataQuery : ICollectDataQuery
    {
        private readonly IGetRecentProjectsQuery _getRecentProjectsQuery;
        private readonly IGetCrmAppointmentsQuery _getCrmAppointmentsQuery;

        public CollectDataQuery(
            IGetRecentProjectsQuery getRecentProjectsQuery,
            IGetCrmAppointmentsQuery getCrmAppointmentsQuery)
        {
            _getRecentProjectsQuery = getRecentProjectsQuery;
            _getCrmAppointmentsQuery = getCrmAppointmentsQuery;
        }

        public async Task<CollectDataModel> Execute(CollectDataRequest request)
        {
            return new CollectDataModel
            {
                EmpID = request.EmpID,
                Date = request.Date,
                CrmAppointments = await _getCrmAppointmentsQuery.Execute(new GetCrmAppointmentsRequest(
                    request.TenantUrl,
                    request.EmpID,
                    request.Date,
                    request.Date,
                    request.Token)),
                RecentProjects = await _getRecentProjectsQuery.Execute(new GetRecentProjectsRequest(
                    request.TenantUrl,
                    request.EmpID,
                    request.Token))
            };
        }
    }

    public interface ICollectDataQuery
    {
        Task<CollectDataModel> Execute(CollectDataRequest request);
    }
}
