using FluentAssertions;
using Newtonsoft.Json;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.AzureDevOps;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.Crm;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.RecentProjects;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.GetTimesheets;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.SuggestTimeSheet;
using System.Collections.Generic;
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

            json = File.ReadAllText("Data/timepro-api-commits.json");
            var commits = JsonConvert.DeserializeObject<GitCommitResult>(json);

            appointments.Should().NotBeEmpty();
            commits.data.Should().NotBeEmpty();

            var query = new SuggestTimeSheetQuery();
            var request = new SuggestTimeSheetRequest
            {
                Date = "2019-05-07+10",
                EmpID = "JEK",
                CrmAppointments = appointments,
                RecentProjects = recentProjects,
                Commits = commits.data
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
            result.Comment.Should().Be($"Commits:\n" +
                $"- Added perf test proejct.\n" +
                $"- Added test for generating seed data.\n" +
                $"- Moved perf project from logical folder PL to BL.\n" +
                $"- Moved Eplan.ManagementService.Application from root to logical folder BL.\n" +
                $"- Connect to the DB test.\n" +
                $"- Moved EfCulkSeedPersister to data layer.\n" +
                $"- Completed EfBulkSeedPersister refactor to data layer.\n" +
                $"- Connect to DB test now clears all tables and verifies if it can execute a SQL statement.\n" +
                $"- Added test that do a full seeding process.\n" +
                $"- Added DB fixture and converted one test.\n" +
                $"- Completed seeding DB and making sure we reuse the DB.\n" +
                $"- First \"real\" test.");
        }

        // Sophie is currently not supported as it can take almost 2 minutes to get results from Azure DevOps.
        [Theory]
        [InlineData("Data/timepro-api-commits-timepro.json", "2019-04-18", "TP", "WEBDEV", "Commits:\n- Added invoice templates.\n- Fixed SharePoint connectivity issue from local machine.\n- Ignore .angulardocs.json")]
        [InlineData("Data/timepro-api-commits-empty.json", "2019-03-12", null, null, null)]
        //[InlineData("Data/timepro-api-commits-sophie.json", "2019-03-14", "GVOUF1", "WEBDEV", "Commits:\n- Added default timezone as a configuration\n- Minor refactor of the name\n- Added storybook\n- Added more storybooks")]
        public async Task ShouldSuggestInternalWorkTimeSheet(string gitCommitsFile, string date, string projectId, string categoryId, string comment)
        {
            var json = File.ReadAllText("Data/timepro-api-crm-mixed-client-leave.json");
            var appointments = JsonConvert.DeserializeObject<CrmAppointmentModel[]>(json);

            json = File.ReadAllText("Data/timepro-api-recent-projects.json");
            var recentProjects = JsonConvert.DeserializeObject<RecentProjectModel[]>(json);

            json = File.ReadAllText(gitCommitsFile);
            var commits = JsonConvert.DeserializeObject<GitCommitResult>(json);

            appointments.Should().NotBeEmpty();

            var query = new SuggestTimeSheetQuery();
            var request = new SuggestTimeSheetRequest
            {
                Date = date + "+10",
                EmpID = "JEK",
                CrmAppointments = appointments,
                RecentProjects = recentProjects,
                Commits = commits.data
            };

            var result = await query.Execute(request);

            result.Should().NotBeNull();
            result.EmpID.Should().Be("JEK");
            result.ClientID.Should().Be("SSW");
            result.ProjectID.Should().Be(projectId);
            result.CategoryID.Should().Be(categoryId);
            result.BillableID.Should().Be("W");
            result.LocationID.Should().Be("SSW");
            result.DateCreated.Should().Be(date);
            result.Comment.Should().Be(comment);
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
            result.AlreadyHasTimesheet.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldAlreadyHaveTimesheet()
        {
            string date = "2019-05-06";
            string categoryId = "LNWD";
            string comment = "SSW (SSW) - Labour Day public holiday (QLD only)";

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
                RecentProjects = recentProjects,
                Timesheets = new List<TimesheetModel>
                {
                    new TimesheetModel
                    {
                        id = 729994442,
                        title = "SSW (SSW) - Labour Day public holiday (QLD only)"
                    }
                }
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
            result.AlreadyHasTimesheet.Should().BeTrue();
        }
    }
}
