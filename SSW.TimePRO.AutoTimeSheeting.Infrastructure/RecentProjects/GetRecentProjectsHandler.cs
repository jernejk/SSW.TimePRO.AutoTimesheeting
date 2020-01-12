using Flurl;
using Flurl.Http;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.RecentProjects
{
    public class GetRecentProjectsHandler : IGetRecentProjectsQuery, IRequestHandler<GetRecentProjects, RecentProjectModel[]>
    {
        public Task<RecentProjectModel[]> Execute(GetRecentProjects request)
        {
            return Handle(request, default);
        }

        public async Task<RecentProjectModel[]> Handle(GetRecentProjects request, CancellationToken cancellationToken)
        {
            var url = new Url(request.TenantUrl)
                .AppendPathSegment("/Ajax/GetPreviousProjects")
                .SetQueryParam("empID", request.EmpID)
                // TimePro bug: Basic auth is not base64 decoded on the server and takes the raw token.
                .WithHeader("Authorization", $"Basic {request.Token}");

            return await url.GetJsonAsync<RecentProjectModel[]>(cancellationToken);
        }
    }

    public interface IGetRecentProjectsQuery
    {
        Task<RecentProjectModel[]> Execute(GetRecentProjects request);
    }
}
