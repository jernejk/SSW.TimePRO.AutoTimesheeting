using SSW.TimePRO.AutoTimeSheeting.Infrastructure.Crm;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.RecentProjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets
{
    public class SuggestTimeSheetRequest
    {
        public string EmpID { get; set; }
        public string Date { get; set; }
        public IEnumerable<RecentProjectModel> RecentProjects { get; set; }
        public IEnumerable<CrmAppointmentModel> CrmAppointments { get; set; }
    }
}
