namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.AzureDevOps
{
    public class GetCommitsByEmpIDRequest : BaseTimeProRequest
    {
        public GetCommitsByEmpIDRequest(string tenantUrl, string empId, string date, string token)
            : base(tenantUrl, token)
        {
            EmpID = empId;
            Date = date;
        }

        public string EmpID { get; set; }
        public string Date { get; set; }
    }
}
