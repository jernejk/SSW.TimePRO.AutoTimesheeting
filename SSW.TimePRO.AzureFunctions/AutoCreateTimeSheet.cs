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

            date = date?.Replace(" ", "+");

            var collectDataRequest = new CollectDataRequest(tenantUrl, empID, date, token);
            var collectedData = await collectDataQuery.Execute(collectDataRequest);

            var timesheet = await suggestTimeSheetQuery.Execute(collectedData.ToSuggestTimeSheetRequest());
            if (timesheet.ClientID == null || timesheet.CategoryID == null || timesheet.ProjectID == null)
            {
                var model = new ModelStateDictionary();
                model.AddModelError("timesheet", "No valid time sheet was found");
                return new BadRequestObjectResult(model);
            }

            var rate = await getClientRateQuery.Execute(new GetClientRateRequest(tenantUrl, empID, timesheet.ClientID, token));
            if (rate == null)
            {
                var model = new ModelStateDictionary();
                model.AddModelError("rate", "Rate is not available");
                return new BadRequestObjectResult(model);
            }

            var request = new CreateTimeSheetRequest(tenantUrl, timesheet, rate.Value, token);
            var result = await createTimeSheetCommand.Execute(request);

            return new JsonResult(result);
        }
    }
}
