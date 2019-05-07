using SSW.TimePRO.AutoTimeSheeting.Infrastructure.AzureDevOps;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.Crm;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.RecentProjects;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.SuggestTimeSheet;
using System.Collections.Generic;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.CollectData
{
    public class CollectDataModel
    {
        public string EmpID { get; set; }
        public string Date { get; set; }
        public IEnumerable<RecentProjectModel> RecentProjects { get; set; }
        public IEnumerable<CrmAppointmentModel> CrmAppointments { get; set; }
        public IEnumerable<GitCommitModel> Commits { get; set; }

        public SuggestTimeSheetRequest ToSuggestTimeSheetRequest()
            => new SuggestTimeSheetRequest
            {
                EmpID = EmpID,
                Date = Date,
                RecentProjects = RecentProjects,
                CrmAppointments = CrmAppointments,
                Commits = Commits
            };
    }
}
