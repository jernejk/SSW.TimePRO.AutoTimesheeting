using Flurl;
using Flurl.Http;
using System.Threading.Tasks;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.ClientRate
{
    public class GetClientRateQuery : IGetClientRateQuery
    {
        public async Task<decimal?> Execute(GetClientRateRequest request)
        {
            var url = new Url(request.TenantUrl)
                .AppendPathSegment("/Ajax/GetClientRate")
                .SetQueryParam("empID", request.EmpID)
                .SetQueryParam("clientID", request.ClientID)
                .WithBasicAuth(request.Token, string.Empty);

            string rawRate = await url.GetStringAsync();

            // Default empty value is "".
            rawRate = rawRate?.Trim('"');

            bool canParse = decimal.TryParse(rawRate, out var rate);
            return canParse ? rate : (decimal?)null;
        }
    }

    public interface IGetClientRateQuery
    {
        Task<decimal?> Execute(GetClientRateRequest request);
    }
}
