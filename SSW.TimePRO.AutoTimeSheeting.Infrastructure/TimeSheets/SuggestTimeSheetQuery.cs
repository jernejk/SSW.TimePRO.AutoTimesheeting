using System;
using System.Linq;
using System.Threading.Tasks;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets
{
    public class SuggestTimeSheetQuery : ISuggestTimeSheetQuery
    {
        public Task<SuggestTimeSheetModel> Execute(SuggestTimeSheetRequest request)
        {
            var appointment = request.CrmAppointments.FirstOrDefault(a => a.start.StartsWith(request.Date));
            var project = request.RecentProjects.FirstOrDefault(a => a.ClientID.Equals(appointment?.clientId, StringComparison.OrdinalIgnoreCase));
            var result = new SuggestTimeSheetModel
            {
                EmpID = request.EmpID,
                ClientID = appointment?.clientId,
                ProjectID = project?.ProjectID,
                CategoryID = project?.CategoryID,
                BillableID = project?.ClientID != null && project?.ClientID.Equals("SSW") != true ? "B" : "W",
                LocationID = "SSW",
                DateCreated = request.Date
            };

            return Task.FromResult(result);
        }
    }

    public interface ISuggestTimeSheetQuery
    {
        Task<SuggestTimeSheetModel> Execute(SuggestTimeSheetRequest request);
    }
}
