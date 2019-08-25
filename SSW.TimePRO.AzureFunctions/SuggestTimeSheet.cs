using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.CollectData;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.SuggestTimeSheet;
using System;
using System.Threading.Tasks;

namespace SSW.TimePRO.AzureFunctions
{
    public class SuggestTimeSheet
    {
        private readonly ICollectDataQuery _collectDataQuery;
        private readonly ISuggestTimeSheetQuery _suggestTimeSheetQuery;

        public SuggestTimeSheet(
            ICollectDataQuery collectDataQuery,
            ISuggestTimeSheetQuery suggestTimeSheetQuery)
        {
            _collectDataQuery = collectDataQuery;
            _suggestTimeSheetQuery = suggestTimeSheetQuery;
        }

        [FunctionName("SuggestTimeSheet")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
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
            var collectedData = await _collectDataQuery.Execute(collectDataRequest);

            var result = await _suggestTimeSheetQuery.Execute(collectedData.ToSuggestTimeSheetRequest());
            return new JsonResult(result);
        }
    }
}
