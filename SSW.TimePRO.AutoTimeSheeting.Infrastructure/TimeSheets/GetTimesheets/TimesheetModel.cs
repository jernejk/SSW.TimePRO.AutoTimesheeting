using System;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.GetTimesheets
{
    public class TimesheetModel
    {
        public int id { get; set; }
        public string title { get; set; }
        public bool allDay { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string color { get; set; }
        public string textColor { get; set; }
    }

}
