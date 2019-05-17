using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.SuggestTimeSheet;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.CreateTimeSheet
{
    public class CreateTimeSheetRequest : BaseTimeProRequest
    {
        public CreateTimeSheetRequest(string tenantUrl, string token)
            : base(tenantUrl, token)
        {
        }

        public CreateTimeSheetRequest(string tenantUrl, SuggestTimeSheetModel timesheet, decimal clientEmpRate, decimal clientTaxRate, string token)
            : this(tenantUrl, token)
        {
            EmpID = timesheet.EmpID;
            DateCreated = timesheet.DateCreated;
            ClientID = timesheet.ClientID;
            ProjectID = timesheet.ProjectID;
            CategoryID = timesheet.CategoryID;
            LocationID = timesheet.LocationID;
            BillableID = timesheet.BillableID;
            ClientEmpRate = clientEmpRate;
            ClientTaxRate = clientTaxRate;
            Comment = timesheet.Comment;
        }

        public string EmpID { get; set; }
        public string DateCreated { get; set; }
        public string ClientID { get; set; }
        public string ProjectID { get; set; }
        public string CategoryID { get; set; }
        public string LocationID { get; set; }
        public string BillableID { get; set; }
        public decimal ClientEmpRate { get; set; }
        public decimal ClientTaxRate { get; set; }
        public string Comment { get; set; }
    }
}
