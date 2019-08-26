using Flurl;
using Flurl.Http;
using System.Threading.Tasks;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.ClientRate
{
    public class GetClientRateQuery : IGetClientRateQuery
    {
        public async Task<ClientRateModel> Execute(GetClientRateRequest request)
        {
            var clientEmpRateRequest = new Url(request.TenantUrl)
                .AppendPathSegment("/Ajax/GetClientRate")
                .SetQueryParam("empID", request.EmpID)
                .SetQueryParam("clientID", request.ClientID)
                // TimePro bug: Basic auth is not base64 decoded on the server and takes the raw token.
                .WithHeader("Authorization", $"Basic {request.Token}");

            var clientTaxRateRequest = new Url(request.TenantUrl)
                .AppendPathSegment("Ajax/GetClientTaxRate")
                .SetQueryParam("clientID", request.ClientID)
                .SetQueryParam("timesheetID", 0)
                // TimePro bug: Basic auth is not base64 decoded on the server and takes the raw token.
                .WithHeader("Authorization", $"Basic {request.Token}");

            var clientEmpRateTask = GetRateFromRequest(clientEmpRateRequest);
            var clientTaxRateTask = GetRateFromRequest(clientTaxRateRequest);
            return new ClientRateModel
            {
                ClientEmpRate = await clientEmpRateTask,
                ClientTaxRate = await clientTaxRateTask
            };
        }

        private static async Task<decimal?> GetRateFromRequest(IFlurlRequest request)
        {
            string rawRate = await request.GetStringAsync();

            // Default empty value is "".
            rawRate = rawRate?.Trim('"');

            bool canParse = decimal.TryParse(rawRate, out var rate);
            return canParse ? rate : (decimal?)null;
        }
    }

    public interface IGetClientRateQuery
    {
        Task<ClientRateModel> Execute(GetClientRateRequest request);
    }
}
