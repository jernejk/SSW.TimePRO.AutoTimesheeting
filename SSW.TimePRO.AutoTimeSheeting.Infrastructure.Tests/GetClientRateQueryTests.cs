using FluentAssertions;
using Flurl.Http.Testing;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.ClientRate;
using System.Threading.Tasks;
using Xunit;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.Tests
{
    public class GetClientRateQueryTests
    {
        [Fact]
        public async Task ShouldReturnRate()
        {
            var query = new GetClientRateQuery();
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith("245", 200);
                httpTest.RespondWith("0.100000", 200);

                var request = new GetClientRateRequest("http://tntUrl.xyz", "JEK", "SSW", "token-1");
                var result = await query.Execute(request);

                result.Should().NotBeNull("both rates are available");
                result.ClientEmpRate.Should().Be(245m, "service is returning \"245\" for client rate as JSON-ish single number.");
                result.ClientTaxRate.Should().Be(0.1m, "service is returning \"0.10000\" for client tax rate as JSON-ish single number.");
                httpTest.CallLog.Should().HaveCount(2, "should not fail");
                httpTest
                    .ShouldHaveCalled("http://tntUrl.xyz/Ajax/GetClientRate?empID=JEK&clientID=SSW")
                    .WithBasicAuth("token-1", string.Empty)
                    .Times(1);

                httpTest
                    .ShouldHaveCalled("http://tntUrl.xyz/Ajax/GetClientTaxRate?clientID=SSW&timesheetID=0")
                    .WithBasicAuth("token-1", string.Empty)
                    .Times(1);
            }
        }

        [Theory]
        [InlineData("\"\"", "\"\"", null, null)]
        [InlineData("\"132.5\"", "\"\"", 132.5, null)]
        [InlineData("\"132.5\"", "\"0\"", 132.5, 0)]
        [InlineData("\"\"", "\"0.10000\"", null, 0.1)]
        public async Task ShouldReturnNull(string clientRateString, string clientTaxString, double? clientRate, double? clientTaxRate)
        {
            var query = new GetClientRateQuery();
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(clientRateString, 200);
                httpTest.RespondWith(clientTaxString, 200);

                var request = new GetClientRateRequest("http://tntUrl.xyz", "JEK", "SSW", "token-1");
                var result = await query.Execute(request);

                result.Should().NotBeNull("even if both rates are null, this should not be null");
                result.ClientEmpRate.Should().Be((decimal?)clientRate, "service is returning \"\" and is parsed as null.");
                result.ClientTaxRate.Should().Be((decimal?)clientTaxRate, "service is returning \"\" and is parsed as null.");
                httpTest.CallLog.Should().HaveCount(2, "should not fail");
                httpTest
                    .ShouldHaveCalled("http://tntUrl.xyz/Ajax/GetClientRate?empID=JEK&clientID=SSW")
                    .WithBasicAuth("token-1", string.Empty)
                    .Times(1);
            }
        }
    }
}
