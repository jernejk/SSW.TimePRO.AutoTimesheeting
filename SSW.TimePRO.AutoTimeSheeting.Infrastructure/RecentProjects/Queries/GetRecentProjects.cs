using Flurl;
using Flurl.Http;
using System.Threading.Tasks;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.RecentProjects.Queries
{
    public class GetRecentProjects
    {
        public Task<RecentProjectModel[]> Execute(string tenantUrl, string empID, string token)
        {
            var url = new Url(tenantUrl)
                .AppendPathSegment("/Ajax/GetPreviousProjects")
                .SetQueryParam("empID", empID)
                .WithBasicAuth(token, string.Empty);

            return url.GetJsonAsync<RecentProjectModel[]>();
        }
    }
}
