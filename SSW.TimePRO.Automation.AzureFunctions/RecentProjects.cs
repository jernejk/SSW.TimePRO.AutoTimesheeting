using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.RecentProjects;
using MediatR;
using System.Threading;

namespace SSW.TimePRO.Automation.AzureFunctions
{
    public class RecentProjects : BaseAzureFunctions
    {
        public RecentProjects(IMediator mediator)
            : base(mediator) { }

        [FunctionName("RecentProjects")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            GetRecentProjects request,
            ILogger log,
            CancellationToken ct)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return  await RunRequest(request, ct);
        }

        [FunctionName("RecentProjectsEvent")]
        public async Task<RecentProjectModel[]> Trigger(
            [ActivityTrigger]
            GetRecentProjects request,
            ILogger log,
            CancellationToken ct)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return await Mediator.Send(request, ct);
        }
    }
}
