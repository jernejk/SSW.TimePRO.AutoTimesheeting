using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AzureFunctions.Autofac;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.GetTimesheets;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SSW.TimePRO.AzureFunctions
{
    [DependencyInjectionConfig(typeof(DIConfig))]
    public static class TimeSheets
    {
        [FunctionName("TimeSheets")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            [Inject] IGetTimesheetsQuery query)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string tenantUrl = req.Query["tenantUrl"];
            string empID = req.Query["empID"];
            string startRaw = req.Query["start"];
            string endRaw = req.Query["end"];
            string token = req.Query["token"];

            // We still want to validate dates.
            DateTime.TryParse(startRaw, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var start);
            DateTime.TryParse(endRaw, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var end);

            var validationModel = Validate(tenantUrl, empID, start, end, token);
            if (!validationModel.IsValid)
            {
                return new BadRequestObjectResult(validationModel);
            }

            var result = await query.Execute(new GetTimesheetsRequest(tenantUrl, empID, startRaw, endRaw, token));

            return new JsonResult(result);
        }

        private static ModelStateDictionary Validate(string tenantUrl, string empID, DateTime start, DateTime end, string token)
        {
            var model = new ModelStateDictionary();
            if (string.IsNullOrEmpty(tenantUrl))
            {
                model.AddModelError(nameof(tenantUrl), "Tenant URL must not be empty");
            }

            if (string.IsNullOrEmpty(empID))
            {
                model.AddModelError(nameof(empID), "Employee ID must not be empty");
            }

            if (string.IsNullOrEmpty(token))
            {
                model.AddModelError(nameof(token), "Token must not be empty");
            }

            if (start == DateTime.MinValue)
            {
                model.AddModelError(nameof(start), "Start date is not in validate format");
            }

            if (end == DateTime.MinValue)
            {
                model.AddModelError(nameof(end), "Start date is not in validate format");
            }

            return model;
        }
    }
}
