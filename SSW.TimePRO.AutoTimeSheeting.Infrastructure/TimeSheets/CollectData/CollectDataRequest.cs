namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.CollectData
{
    public class CollectDataRequest : BaseTimeProRequest
    {
        public CollectDataRequest(string tenantUrl, string empId, string date, string token)
            : base(tenantUrl, token)
        {
            EmpID = empId;
            Date = date;
        }

        public string EmpID { get; set; }
        public string Date { get; set; }
    }
}
