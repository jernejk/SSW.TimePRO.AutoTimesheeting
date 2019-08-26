namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.CollectData
{
    public class CollectDataRequest : BaseTimeProRequest
    {
        public CollectDataRequest(string tenantUrl, string empId, string date, string token, string githubUsername, string githubToken)
            : base(tenantUrl, token)
        {
            EmpID = empId;
            Date = date;
            GitHubUsername = githubUsername;
            GitHubToken = githubToken;
        }

        public string EmpID { get; set; }
        public string Date { get; set; }
        public string GitHubUsername { get; set; }
        public string GitHubToken { get; set; }
    }
}
