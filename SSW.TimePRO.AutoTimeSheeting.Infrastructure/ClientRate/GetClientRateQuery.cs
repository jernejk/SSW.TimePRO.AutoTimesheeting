using Flurl;
using Flurl.Http;
using System.Threading.Tasks;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.ClientRate
{
    public class GetClientRateQuery : IGetClientRateQuery
    {
        public async Task<decimal?> Execute(string tenantUrl, string empID, string clientID, string token)
        {
            var url = new Url(tenantUrl)
                .AppendPathSegment("/api/ClientRate")
                .SetQueryParam("empID", empID)
                .SetQueryParam("clientID", clientID)
                .WithBasicAuth(token, string.Empty);

            string rawRate = await url.GetStringAsync();

            // Default empty value is "".
            rawRate = rawRate?.Trim('"');

            bool canParse = decimal.TryParse(rawRate, out var rate);
            return canParse ? rate : (decimal?)null;
        }
    }

    public interface IGetClientRateQuery
    {
        Task<decimal?> Execute(string tenantUrl, string empID, string clientID, string token);
    }
}
