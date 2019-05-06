using FluentAssertions;
using Newtonsoft.Json;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.Crm;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.RecentProjects;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.SuggestTimeSheet;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.Tests
{
    public class SuggestTimeSheetQueryTests
    {
        [Fact]
        public async Task ShouldSuggestBillableTimeSheet()
        {
            var json = File.ReadAllText("Data/timepro-api-crm-mixed-client-leave.json");
            var appointments = JsonConvert.DeserializeObject<CrmAppointmentModel[]>(json);

            json = File.ReadAllText("Data/timepro-api-recent-projects.json");
            var recentProjects = JsonConvert.DeserializeObject<RecentProjectModel[]>(json);

            appointments.Should().NotBeEmpty();

            var query = new SuggestTimeSheetQuery();
            var request = new SuggestTimeSheetRequest
            {
                Date = "2019-05-07+10",
                EmpID = "JEK",
                CrmAppointments = appointments,
                RecentProjects = recentProjects
            };

            var result = await query.Execute(request);

            result.Should().NotBeNull();
            result.EmpID.Should().Be("JEK");
            result.ClientID.Should().Be("T2VV5F");
            result.ProjectID.Should().Be("DG8WXY");
            result.CategoryID.Should().Be("WEBDEV");
            result.BillableID.Should().Be("B");
            result.LocationID.Should().Be("SSW");
            result.DateCreated.Should().Be("2019-05-07");
            result.Comment.Should().BeNull();
        }

        [Theory]
        [InlineData("2019-05-06", "LNWD", "SSW (SSW) - Labour Day public holiday (QLD only)")]
        [InlineData("2019-04-23", "L-ANN", "Annual leave")]
        [InlineData("2019-05-02", "LSICK", "Sick leave")]
        public async Task ShouldSuggestLeaveTimeSheet(string date, string categoryId, string comment)
        {
            var json = File.ReadAllText("Data/timepro-api-crm-mixed-client-leave.json");
            var appointments = JsonConvert.DeserializeObject<CrmAppointmentModel[]>(json);

            json = File.ReadAllText("Data/timepro-api-recent-projects.json");
            var recentProjects = JsonConvert.DeserializeObject<RecentProjectModel[]>(json);

            appointments.Should().NotBeEmpty();

            var query = new SuggestTimeSheetQuery();
            var request = new SuggestTimeSheetRequest
            {
                Date = date,
                EmpID = "JEK",
                CrmAppointments = appointments,
                RecentProjects = recentProjects
            };

            var result = await query.Execute(request);

            result.Should().NotBeNull();
            result.EmpID.Should().Be("JEK");
            result.ClientID.Should().Be("SSW");
            result.ProjectID.Should().Be("LEAVE");
            result.CategoryID.Should().Be(categoryId);
            result.BillableID.Should().Be("W");
            result.LocationID.Should().Be("SSW");
            result.DateCreated.Should().Be(date);
            result.Comment.Should().Be(comment);
        }
    }
}
