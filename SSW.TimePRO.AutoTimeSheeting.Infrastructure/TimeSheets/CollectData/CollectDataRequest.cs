using SSW.TimePRO.AutoTimeSheeting.Infrastructure.AzureDevOps;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.Crm;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.GitHub;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.RecentProjects;

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

        public GetCrmAppointments ToCrmAppointmentsRequest()
        {
            return new GetCrmAppointments(TenantUrl, EmpID, Date, Date, Token);
        }

        public GetRecentProjects ToRecentProjectsRequest()
        {
            return new GetRecentProjects(TenantUrl, EmpID, Token);
        }

        public GetGitHubCommits ToGitHubCommitsRequest()
        {
            return new GetGitHubCommits
            {
                Username = GitHubUsername,
                Date = Date,
                Token = GitHubToken
            };
        }

        public GetCommitsByEmpID ToGetCommitsByEmpIDRequest()
        {
            return new GetCommitsByEmpID(TenantUrl, EmpID, Date, Token);
        }

        public GetTimesheets.GetTimesheets ToTimesheetsRequest()
        {
            return new GetTimesheets.GetTimesheets(
                TenantUrl,
                EmpID,
                Date,
                Date,
                Token);
        }
    }
}
