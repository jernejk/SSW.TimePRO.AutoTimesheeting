namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.SuggestTimeSheet
{
    public class SuggestTimeSheetModel
    {
        public string EmpID { get; set; }
        public string DateCreated { get; set; }
        public string ClientID { get; set; }
        public string ProjectID { get; set; }
        public string CategoryID { get; set; }
        public string LocationID { get; set; }
        public string BillableID { get; set; }
        public string Comment { get; set; }
    }
}
