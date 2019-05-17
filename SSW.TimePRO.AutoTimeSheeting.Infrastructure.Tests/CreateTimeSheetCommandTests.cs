using FluentAssertions;
using Flurl.Http.Testing;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.CreateTimeSheet;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.Tests
{
    public class CreateTimeSheetCommandTests
    {
        [Fact(Skip = "It's not timezone agnostic")]
        public async Task ShouldAddTimeSheet()
        {
            var command = new CreateTimeSheetCommand();
            using (var httpTest = new HttpTest())
            {
                string response = File.ReadAllText("Data/timepro-web-timesheet-added.html");
                httpTest.RespondWith(response, 200);

                string empID = "JEK";
                string date = "2019-05-06";
                var request = new CreateTimeSheetRequest("http://tnturl.xyz", "token-1")
                {
                    EmpID = empID,
                    DateCreated = date + "+10",
                    ClientID = "SSW",
                    ProjectID = "LEAVE",
                    CategoryID = "LNWD",
                    LocationID = "SSW",
                    BillableID = "W",
                    ClientEmpRate = 245m,
                    ClientTaxRate = 0.1m,
                    Comment = "SSW (SSW) - Labour Day public holiday (QLD & SI only)"
                };

                var result = await command.Execute(request);

                result.Should().NotBeNull();
                result.IsSuccessful.Should().BeTrue();

                httpTest.CallLog.Should().HaveCount(1, "should not fail");
                var httpRequest = httpTest.CallLog.First();
                httpRequest.Request.RequestUri.ToString().Should().Be($"http://tnturl.xyz/Timesheet/{empID}/{date}/Add");

                httpTest
                    .ShouldHaveCalled($"http://tnturl.xyz/Timesheet/{empID}/{date}/Add")
                    .WithBasicAuth("token-1", string.Empty)
                    .WithVerb(HttpMethod.Post)
                    .WithRequestBody(
                        "EmpTime.TimeID=0&EmpTime.IsAutomaticComments=False&EmpTime.EmpID=JEK" +
                        "&EmpTime.TfsComments=&EmpTime.DateCreated=06%2F05%2F2019&EmpTime.ClientID=SSW&EmpTime.ProjectID=LEAVE" +
                        "&UseIteration=false&EmpTime.CategoryID=LNWD&EmpTime.LocationID=SSW" +
                        "&EmpTime.TimeStart=8%3A00+AM&EmpTime.TimeEnd=5%3A00+PM&EmpTime.TimeLess=1&TotalTimeTextBox=8" +
                        "&EmpTime.BillableID=W&EmpTime.IsBillingTypeOverridden=false&EmpTime.SellPrice=245" +
                        "&EmpTime.SalesTaxPct=0.1&EmpCreated=&EmpUpdated=&SaveType=Add" +
                        "&Note=SSW+(SSW)+-+Labour+Day+public+holiday+(QLD+%26+SI+only)")
                    .Times(1);
            }
        }
    }
}
