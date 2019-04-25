using Flurl;
using Flurl.Http;
using System.Threading.Tasks;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.RecentProjects
{
    public class GetRecentProjectsQuery : IGetRecentProjectsQuery
    {
        public Task<RecentProjectModel[]> Execute(GetRecentProjectsRequest request)
        {
            var url = new Url(request.TenantUrl)
                .AppendPathSegment("/Ajax/GetPreviousProjects")
                .SetQueryParam("empID", request.EmpID)
                .WithBasicAuth(request.Token, string.Empty);

            return url.GetJsonAsync<RecentProjectModel[]>();
        }
    }

    public interface IGetRecentProjectsQuery
    {
        Task<RecentProjectModel[]> Execute(GetRecentProjectsRequest request);
    }
}
