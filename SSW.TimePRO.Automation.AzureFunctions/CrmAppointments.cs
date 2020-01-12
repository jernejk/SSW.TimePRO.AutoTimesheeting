using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.Crm;
using System.Collections.Generic;
using System.Threading;

namespace SSW.TimePRO.Automation.AzureFunctions
{
    public class CrmAppointments : BaseAzureFunctions
    {
        public CrmAppointments(IMediator mediator)
            : base(mediator) { }

        [FunctionName("CrmAppointments")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
            GetCrmAppointments request,
            ILogger log,
            CancellationToken ct)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return await RunRequest(request, ct);
        }

        [FunctionName("CrmAppointmentsEvent")]
        public async Task<IEnumerable<CrmAppointmentModel>> Trigger(
            [ActivityTrigger]
            GetCrmAppointments request,
            ILogger log,
            CancellationToken ct)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return await Mediator.Send(request, ct);
        }
    }
}
