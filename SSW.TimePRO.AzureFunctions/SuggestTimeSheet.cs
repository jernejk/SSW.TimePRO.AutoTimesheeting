using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AzureFunctions.Autofac;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.CollectData;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.SuggestTimeSheet;

namespace SSW.TimePRO.AzureFunctions
{
    [DependencyInjectionConfig(typeof(DIConfig))]
    public static class SuggestTimeSheet
    {
        [FunctionName("SuggestTimeSheet")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            [Inject] ICollectDataQuery collectDataQuery,
            [Inject] ISuggestTimeSheetQuery suggestTimeSheetQuery)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string tenantUrl = req.Query["tenantUrl"];
            string empID = req.Query["empID"];
            string date = req.Query["date"];
            string token = req.Query["token"];

            if (string.IsNullOrEmpty(date))
            {
                date = DateTime.Today.ToString("yyyy-MM-dd") + "+10";
            }

            date = date?.Replace(" ", "+");

            var collectDataRequest = new CollectDataRequest(tenantUrl, empID, date, token);
            var collectedData = await collectDataQuery.Execute(collectDataRequest);

            var result = await suggestTimeSheetQuery.Execute(collectedData.ToSuggestTimeSheetRequest());
            return new JsonResult(result);
        }
    }
}
