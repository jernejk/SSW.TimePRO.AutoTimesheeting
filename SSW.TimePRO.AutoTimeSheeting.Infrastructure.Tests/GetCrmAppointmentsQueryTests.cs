using FluentAssertions;
using Flurl.Http.Testing;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.Crm;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.Tests
{
    public class GetCrmAppointmentsQueryTests
    {
        [Fact]
        public async Task ShouldReturnRate()
        {
            var query = new GetCrmAppointmentsQuery();
            using (var httpTest = new HttpTest())
            {
                string response = "[{\"id\":\"e5fcf48c-5318-e911-81d2-00155d012b45\",\"start\":\"2019-04-25 09:00:00\",\"end\":\"2019-04-25 18:00:00\",\"title\":\"SSW (SSW) - Anzac Day public holiday (All of Australia)\",\"allDay\":false,\"editable\":false,\"clientId\":\"SSW\",\"projectId\":\"\",\"iterationId\":null}]";
                httpTest.RespondWith(response, 200);

                var request = new GetCrmAppointmentsRequest(
                    "http://tntUrl.xyz",
                    "JEK",
                    new DateTime(2019, 4, 21, 14, 0, 0, DateTimeKind.Utc),
                    new DateTime(2019, 4, 26, 14, 0, 0, DateTimeKind.Utc),
                    "token-1");
                var result = await query.Execute(request);

                result.Should().HaveCount(1, "raw JSON response has only one response");
                httpTest.CallLog.Should().HaveCount(1, "should not fail");
                httpTest
                    .ShouldHaveCalled("http://tntUrl.xyz/Crm/Appointments?employeeID=JEK&start=1555855200&end=1556287200")
                    .WithBasicAuth("token-1", string.Empty)
                    .Times(1);
            }
        }

        [Fact]
        public async Task ShouldReturnFailDueIncorrectDateKind()
        {
            var query = new GetCrmAppointmentsQuery();
            using (var httpTest = new HttpTest())
            {
                var request = new GetCrmAppointmentsRequest(
                    "http://tntUrl.xyz",
                    "JEK",
                    new DateTime(2019, 4, 21, 14, 0, 0, DateTimeKind.Utc),
                    new DateTime(2019, 4, 26, 14, 0, 0, DateTimeKind.Local),
                    "token-1");

                Func<Task> action = async () => await query.Execute(request);
                await action.Should().ThrowAsync<ArgumentException>("date is not correct kind");

                httpTest.CallLog.Should().BeEmpty("due validation it should never run");
            }
        }
    }
}
