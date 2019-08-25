using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.ClientRate;
using System.Threading.Tasks;

namespace SSW.TimePRO.AzureFunctions
{
    public class ClientRate
    {
        private readonly IGetClientRateQuery _getClientRateQuery;

        public ClientRate(IGetClientRateQuery getClientRateQuery)
        {
            _getClientRateQuery = getClientRateQuery;
        }

        [FunctionName("ClientRate")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string tenantUrl = req.Query["tenantUrl"];
            string empID = req.Query["empID"];
            string clientID = req.Query["clientID"];
            string token = req.Query["token"];

            var validationModel = Validate(tenantUrl, empID, clientID, token);
            if (!validationModel.IsValid)
            {
                return new BadRequestObjectResult(validationModel);
            }

            var request = new GetClientRateRequest(tenantUrl, empID, clientID, token);
            var result = await _getClientRateQuery.Execute(request);

            return new JsonResult(result);
        }

        private static ModelStateDictionary Validate(string tenantUrl, string empID, string clientID, string token)
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

            if (string.IsNullOrEmpty(clientID))
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
