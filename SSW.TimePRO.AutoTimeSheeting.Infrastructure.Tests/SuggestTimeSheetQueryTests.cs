using FluentAssertions;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.Crm;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.RecentProjects;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.Tests
{
    public class SuggestTimeSheetQueryTests
    {
        [Fact]
        public async Task ShouldSuggestTimeSheet()
        {
            var query = new SuggestTimeSheetQuery();

            var request = new SuggestTimeSheetRequest
            {
                Date = "2019-04-25",
                EmpID = "JEK",
                CrmAppointments = new List<CrmAppointmentModel>
                {
                    new CrmAppointmentModel
                    {
                        id = "e5fcf48c-5318-e911-81d2-00155d012b45",
                        start = "2019-04-25 09:00:00",
                        end = "2019-04-25 18:00:00",
                        title = "SSW (SSW) - Anzac Day public holiday (All of Australia)",
                        allDay = false,
                        editable = false,
                        clientId = "SSW",
                        projectId = "",
                        iterationId = null
                    }
                },
                RecentProjects = new List<RecentProjectModel>
                {
                    new RecentProjectModel
                    {
                        Client = "SSWTest",
                        ClientID = "SSW",
                        Project = "Non-working day (e.g. Leave)",
                        ProjectID = "LEAVE",
                        Iteration = null,
                        IterationId = null,
                        Category = "Non-working day - Public Holiday",
                        CategoryID = "LNWD",
                        DateCreated = new DateTime(2015, 3, 2)
                    }
                }
            };

            var result = await query.Execute(request);

            result.Should().NotBeNull();
            result.EmpID.Should().Be("JEK");
            result.ClientID.Should().Be("SSW");
            result.ProjectID.Should().Be("LEAVE");
            result.CategoryID.Should().Be("LNWD");
            result.BillableID.Should().Be("W");
            result.LocationID.Should().Be("SSW");
            result.DateCreated.Should().Be("2019-04-25");
        }
    }
}
