using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.GetTimesheets;

namespace SSW.TimePRO.Automation.AzureFunctions
{
    public class Timesheets : BaseAzureFunctions
    {
        public Timesheets(IMediator mediator)
            : base(mediator) { }

        [FunctionName("Timesheets")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            GetTimesheets request,
            ILogger log,
            CancellationToken ct)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return await RunRequest(request, ct);
        }

        [FunctionName("TimesheetsEvent")]
        public async Task<IEnumerable<TimesheetModel>> Trigger(
            [ActivityTrigger]
            GetTimesheets request,
            ILogger log,
            CancellationToken ct)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return await Mediator.Send(request, ct);
        }
    }
}
