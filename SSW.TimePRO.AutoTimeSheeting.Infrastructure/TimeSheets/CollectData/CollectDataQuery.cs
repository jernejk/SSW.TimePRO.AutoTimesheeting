using SSW.TimePRO.AutoTimeSheeting.Infrastructure.AzureDevOps;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.Crm;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.GitHub;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.RecentProjects;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.GetTimesheets;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.CollectData
{
    public class CollectDataQuery : ICollectDataQuery
    {
        private readonly IGetRecentProjectsQuery _getRecentProjectsQuery;
        private readonly IGetCrmAppointmentsQuery _getCrmAppointmentsQuery;
        private readonly IGetCommitsByEmpIDQuery _getCommitsByEmpIDQuery;
        private readonly IGetGitHubCommitsQuery _getGitHubCommitsQuery;
        private readonly IGetTimesheetsQuery _getTimesheetsQuery;

        public CollectDataQuery(
            IGetRecentProjectsQuery getRecentProjectsQuery,
            IGetCrmAppointmentsQuery getCrmAppointmentsQuery,
            IGetCommitsByEmpIDQuery getCommitsByEmpIDQuery,
            IGetGitHubCommitsQuery getGitHubCommitsQuery,
            IGetTimesheetsQuery getTimesheetsQuery)
        {
            _getRecentProjectsQuery = getRecentProjectsQuery;
            _getCrmAppointmentsQuery = getCrmAppointmentsQuery;
            _getCommitsByEmpIDQuery = getCommitsByEmpIDQuery;
            _getGitHubCommitsQuery = getGitHubCommitsQuery;
            _getTimesheetsQuery = getTimesheetsQuery;
        }

        public async Task<CollectDataModel> Execute(CollectDataRequest request)
        {
            var data = new CollectDataModel
            {
                EmpID = request.EmpID,
                Date = request.Date
            };

            var crmAppointmentsTask = _getCrmAppointmentsQuery.Execute(new GetCrmAppointments(
                    request.TenantUrl,
                    request.EmpID,
                    request.Date,
                    request.Date,
                    request.Token));

            var recentProjectsTask = _getRecentProjectsQuery.Execute(new GetRecentProjects(
                    request.TenantUrl,
                    request.EmpID,
                    request.Token));

            var timesheetsTask = _getTimesheetsQuery.Execute(new GetTimesheets.GetTimesheets(
                    request.TenantUrl,
                    request.EmpID,
                    request.Date,
                    request.Date,
                    request.Token));

            IEnumerable<GitCommitModel> commits = await GetCommits(request);

            data.Commits = commits;

            data.CrmAppointments = await crmAppointmentsTask;
            data.RecentProjects = await recentProjectsTask;
            data.Timesheets = await timesheetsTask;

            return data;
        }

        private async Task<IEnumerable<GitCommitModel>> GetCommits(CollectDataRequest request)
        {
            var gitHubCommitsTask = _getGitHubCommitsQuery.Execute(new GetGitHubCommits
            {
                Username = request.GitHubUsername,
                Token = request.GitHubToken,
                Date = request.Date
            });

            var azureDevOpsCommitsTask = _getCommitsByEmpIDQuery.Execute(new GetCommitsByEmpID(
                                request.TenantUrl,
                                request.EmpID,
                                request.Date,
                                request.Token));

            var gitHubCommits = await gitHubCommitsTask;
            var azureDevOpsCommits = await azureDevOpsCommitsTask;

            var commits = new List<GitCommitModel>(gitHubCommits.data);
            commits.AddRange(azureDevOpsCommits);
            return commits;
        }
    }

    public interface ICollectDataQuery
    {
        Task<CollectDataModel> Execute(CollectDataRequest request);
    }
}
