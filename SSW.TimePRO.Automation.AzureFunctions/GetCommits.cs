using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MediatR;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.GitHub;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.AzureDevOps;
using System.Collections.Generic;
using System.Threading;

namespace SSW.TimePRO.Automation.AzureFunctions
{
    public class GetCommits : BaseAzureFunctions
    {
        public GetCommits(IMediator mediator)
            : base(mediator) { }

        [FunctionName("GetGitHubCommits")]
        public async Task<IActionResult> RunForGitHub(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            GetGitHubCommits request,
            ILogger log,
            CancellationToken ct)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return await RunRequest(request, ct);
        }

        [FunctionName("GetGitHubCommitsEvent")]
        public async Task<GitCommitResult> TriggerForGitHub(
            [ActivityTrigger]
            GetGitHubCommits request,
            ILogger log,
            CancellationToken ct)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return await Mediator.Send(request, ct);
        }

        [FunctionName("GetAzureDevOpsCommits")]
        public async Task<IActionResult> RunForAzureDevOps(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            GetCommitsByEmpID request,
            ILogger log,
            CancellationToken ct)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return await RunRequest(request, ct);
        }

        [FunctionName("GetAzureDevOpsCommitsEvent")]
        public async Task<IEnumerable<GitCommitModel>> TriggerForAzureDevOps(
            [ActivityTrigger]
            GetCommitsByEmpID request,
            ILogger log,
            CancellationToken ct)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return await Mediator.Send(request, ct);
        }
    }
}
