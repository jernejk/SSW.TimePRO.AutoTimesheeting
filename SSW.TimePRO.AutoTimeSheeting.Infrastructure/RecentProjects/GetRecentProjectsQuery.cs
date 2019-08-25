using Flurl;
using Flurl.Http;
using System.Threading.Tasks;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.RecentProjects
{
    public class GetRecentProjectsQuery : IGetRecentProjectsQuery
    {
        public async Task<RecentProjectModel[]> Execute(GetRecentProjectsRequest request)
        {
            var url = new Url(request.TenantUrl)
                .AppendPathSegment("/Ajax/GetPreviousProjects")
                .SetQueryParam("empID", request.EmpID)
                // TimePro bug: Basic auth is not base64 decoded on the server and takes the raw token.
                .WithHeader("Authorization", $"Basic {request.Token}");

            return await url.GetJsonAsync<RecentProjectModel[]>();
        }
    }

    public interface IGetRecentProjectsQuery
    {
        Task<RecentProjectModel[]> Execute(GetRecentProjectsRequest request);
    }
}
