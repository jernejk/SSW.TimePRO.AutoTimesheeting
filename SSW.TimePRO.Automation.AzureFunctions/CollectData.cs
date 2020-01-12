using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.AzureDevOps;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.Crm;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.RecentProjects;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.CollectData;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.GetTimesheets;

namespace SSW.TimePRO.Automation.AzureFunctions
{
    public static class CollectData
    {
        [FunctionName("CollectData")]
        public static async Task<CollectDataModel> RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var request = context.GetInput<CollectDataRequest>();
            var data = new CollectDataModel
            {
                EmpID = request.EmpID,
                Date = request.Date
            };

            Task<GitCommitResult> githubCommitsTask = null;

            var crmTask = context.CallActivityAsync<List<CrmAppointmentModel>>("CrmAppointmentsEvent", request.ToCrmAppointmentsRequest());
            var timesheetsTask = context.CallActivityAsync<List<TimesheetModel>>("TimesheetsEvent", request.ToTimesheetsRequest());
            var recentProjectsTask = context.CallActivityAsync<List<RecentProjectModel>>("RecentProjectsEvent", request.ToRecentProjectsRequest());
            var azureDevOpsCommitsTask = context.CallActivityAsync<List<GitCommitModel>>("GetAzureDevOpsCommitsEvent", request.ToGetCommitsByEmpIDRequest());

            if (!string.IsNullOrEmpty(request.GitHubUsername))
            {
                githubCommitsTask = context.CallActivityAsync<GitCommitResult>("GetGitHubCommitsEvent", request.ToGitHubCommitsRequest());
            }

            data.CrmAppointments = await crmTask;
            data.Timesheets = await timesheetsTask;
            data.RecentProjects = await recentProjectsTask;

            var commits = new List<GitCommitModel>(await azureDevOpsCommitsTask);

            if (githubCommitsTask != null)
            {
                var githubCommits = await githubCommitsTask;
                commits.AddRange(githubCommits.data);
            }

            data.Commits = commits;

            return data;
        }

        [FunctionName("CollectData_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {
            var json = await req.Content.ReadAsStringAsync();
            var request = JsonConvert.DeserializeObject<CollectDataRequest>(json);

            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("CollectData", request);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return await starter.WaitForCompletionOrCreateCheckStatusResponseAsync(req, instanceId, TimeSpan.FromSeconds(120));
        }
    }
}