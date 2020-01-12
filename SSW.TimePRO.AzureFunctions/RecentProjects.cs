using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.RecentProjects;
using System.Threading.Tasks;

namespace SSW.TimePRO.AzureFunctions
{
    public class RecentProjects
    {
        private readonly IGetRecentProjectsQuery _getRecentProjectsQuery;

        public RecentProjects(IGetRecentProjectsQuery getRecentProjectsQuery)
        {
            _getRecentProjectsQuery = getRecentProjectsQuery;
        }

        [FunctionName("RecentProjects")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string tenantUrl = req.Query["tenantUrl"];
            string empID = req.Query["empID"];
            string token = req.Query["token"];

            var validationModel = Validate(tenantUrl, empID, token);
            if (!validationModel.IsValid)
            {
                return new BadRequestObjectResult(validationModel);
            }

            var request = new GetRecentProjects(tenantUrl, empID, token);
            var result = await _getRecentProjectsQuery.Execute(request);

            return new JsonResult(result);
        }

        private static ModelStateDictionary Validate(string tenantUrl, string empID, string token)
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

            return model;
        }
    }
}
