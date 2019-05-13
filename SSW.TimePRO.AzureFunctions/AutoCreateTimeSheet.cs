using AzureFunctions.Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.ClientRate;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.CollectData;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.CreateTimeSheet;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.SuggestTimeSheet;
using SSW.TimePRO.AzureFunctions.Models;
using System;
using System.Threading.Tasks;

namespace SSW.TimePRO.AzureFunctions
{
    [DependencyInjectionConfig(typeof(DIConfig))]
    public static class AutoCreateTimeSheet
    {
        [FunctionName("AutoCreateTimeSheet")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            [Inject] ICollectDataQuery collectDataQuery,
            [Inject] ISuggestTimeSheetQuery suggestTimeSheetQuery,
            [Inject] IGetClientRateQuery getClientRateQuery,
            [Inject] ICreateTimeSheetCommand createTimeSheetCommand)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string tenantUrl = req.Query["tenantUrl"];
            string empID = req.Query["empID"];
            string date = req.Query["date"];
            string token = req.Query["token"];
            bool isDebug = GetTruthy(req.Query["debug"]);
            bool isDryRun = GetTruthy(req.Query["dryRun"]);

            if (string.IsNullOrEmpty(date))
            {
                date = DateTime.Today.ToString("yyyy-MM-dd") + "+10";
            }

            date = date?.Replace(" ", "+");

            var collectDataRequest = new CollectDataRequest(tenantUrl, empID, date, token);
            var collectedData = await collectDataQuery.Execute(collectDataRequest);

            var timesheet = await suggestTimeSheetQuery.Execute(collectedData.ToSuggestTimeSheetRequest());
            if (timesheet.ClientID == null || timesheet.CategoryID == null || timesheet.ProjectID == null)
            {
                var model = new ModelStateDictionary();
                if (timesheet.ClientID == null)
                {
                    model.AddModelError(nameof(timesheet.ClientID), "No client was found");
                }

                if (timesheet.ProjectID == null)
                {
                    model.AddModelError(nameof(timesheet.ProjectID), "No project was found");
                }

                if (timesheet.CategoryID == null)
                {
                    model.AddModelError(nameof(timesheet.CategoryID), "No category was found");
                }

                return new BadRequestObjectResult(model);
            }

            var rate = await getClientRateQuery.Execute(new GetClientRateRequest(tenantUrl, empID, timesheet.ClientID, token));
            if (rate == null)
            {
                var model = new ModelStateDictionary();
                model.AddModelError("rate", "Rate is not available");
                return new BadRequestObjectResult(model);
            }

            var result = new AutoCreateTimeSheetModel
            {
                SuggestTimeSheet = timesheet,
                Rate = rate
            };

            if (isDebug)
            {
                result.Debug = collectedData;
            }

            if (!timesheet.AlreadyHasTimesheet && !isDryRun)
            {
                try
                {
                    var request = new CreateTimeSheetRequest(tenantUrl, timesheet, rate.Value, token);
                    var success = await createTimeSheetCommand.Execute(request);
                    result.IsCreated = success.IsSuccessful;
                }
                catch (Exception e)
                {
                    log.LogError(e, "Something went wrong when adding a timesheet");
                }
            }

            return new JsonResult(result);
        }

        private static bool GetTruthy(string value)
            => !string.IsNullOrEmpty(value) && !value.Equals("false", StringComparison.OrdinalIgnoreCase);
    }
}
