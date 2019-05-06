using FluentAssertions;
using Flurl.Http.Testing;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.RecentProjects;
using System.Threading.Tasks;
using Xunit;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.Tests
{
    public class GetRecentProjectsQueryTests
    {
        [Fact]
        public async Task ShouldRecentProjects()
        {
            var query = new GetRecentProjectsQuery();
            using (var httpTest = new HttpTest())
            {
                string response = "[{\"Client\":\"SSWTest\",\"ClientID\":\"SSW\",\"Project\":\"Non-working day (e.g. Leave)\",\"ProjectID\":\"LEAVE\",\"Iteration\":null,\"IterationId\":null,\"Category\":\"Non-working day - Public Holiday\",\"CategoryID\":\"LNWD\",\"DateCreated\":\"\\/Date(1556200800000)\\/\"}]";
                httpTest.RespondWith(response, 200);

                var request = new GetRecentProjectsRequest("http://tntUrl.xyz", "JEK", "token-1");
                var result = await query.Execute(request);

                result.Should().HaveCount(1, "raw JSON response has only one response");
                httpTest.CallLog.Should().HaveCount(1, "should not fail");
                httpTest
                    .ShouldHaveCalled("http://tntUrl.xyz/Ajax/GetPreviousProjects?empID=JEK")
                    .WithBasicAuth("token-1", string.Empty)
                    .Times(1);
            }
        }

        [Fact]
        public async Task ShouldReturnNull()
        {
            var query = new GetRecentProjectsQuery();
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith("", 200);

                var request = new GetRecentProjectsRequest("http://tntUrl.xyz", "JEK", "token-1");
                var result = await query.Execute(request);

                result.Should().BeNull("empty string results in null");
                httpTest.CallLog.Should().HaveCount(1, "should not fail");
                httpTest
                    .ShouldHaveCalled("http://tntUrl.xyz/Ajax/GetPreviousProjects?empID=JEK")
                    .WithBasicAuth("token-1", string.Empty)
                    .Times(1);
            }
        }
    }
}
