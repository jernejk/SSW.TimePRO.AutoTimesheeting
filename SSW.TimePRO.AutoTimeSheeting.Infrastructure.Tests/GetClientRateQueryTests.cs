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

                var request = new GetClientRateRequest("http://tntUrl.xyz", "JEK", "SSW", "token-1");
                var result = await query.Execute(request);

                result.Should().Be(245m, "service is returning \"245\" as JSON-ish single number.");
                httpTest.CallLog.Should().HaveCount(1, "should not fail");
                httpTest
                    .ShouldHaveCalled("http://tntUrl.xyz/Ajax/GetClientRate?empID=JEK&clientID=SSW")
                    .WithBasicAuth("token-1", string.Empty)
                    .Times(1);
            }
        }

        [Fact]
        public async Task ShouldReturnNull()
        {
            var query = new GetClientRateQuery();
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith("\"\"", 200);

                var request = new GetClientRateRequest("http://tntUrl.xyz", "JEK", "SSW", "token-1");
                var result = await query.Execute(request);

                result.Should().Be(null, "because the service is returning empty double quotes for some reason");
                httpTest.CallLog.Should().HaveCount(1, "should not fail");
                httpTest
                    .ShouldHaveCalled("http://tntUrl.xyz/Ajax/GetClientRate?empID=JEK&clientID=SSW")
                    .WithBasicAuth("token-1", string.Empty)
                    .Times(1);
            }
        }
    }
}
