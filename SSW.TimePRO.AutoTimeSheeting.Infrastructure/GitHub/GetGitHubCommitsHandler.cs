using Flurl;
using Flurl.Http;
using MediatR;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.AzureDevOps;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.GitHub
{
    public class GetGitHubCommitsHandler : IGetGitHubCommitsQuery, IRequestHandler<GetGitHubCommits, GitCommitResult>
    {
        private const string BaseUrl = "https://api.github.com";

        public Task<GitCommitResult> Execute(GetGitHubCommits request)
        {
            return Handle(request, default);
        }

        public async Task<GitCommitResult> Handle(GetGitHubCommits request, CancellationToken cancellationToken)
        {
            DateTimeOffset startDate = DateTimeOffset.Parse(request.Date, CultureInfo.InvariantCulture);
            DateTimeOffset endDate = startDate.AddDays(1);

            var url = new Url(BaseUrl)
                .AppendPathSegment($"users/{request.Username}/events")
                .WithHeader("User-Agent", "Auto-Timesheets Azure Func")
                .WithBasicAuth(request.Username, request.Token)
                .AllowAnyHttpStatus();

            try
            {
                var events = await url.GetJsonAsync<EventData[]>();

                var pushEvents = events
                        .Where(e => e.type == "PushEvent" && (e.payload?.commits?.Any() ?? false) && e.created_at > startDate && e.created_at < endDate)
                        .Select(e => new
                        {
                            Repo = e.repo.name,
                            CreatedAt = e.created_at,
                            Commits = e.payload.commits
                        })
                        .GroupBy(e => e.Repo).ToArray();

                var commits = new List<GitCommitModel>();
                foreach (var pushEvent in pushEvents)
                {
                    // Skip personal repos for now.
                    if (pushEvent.Key.Contains(request.Username))
                    {
                        continue;
                    }

                    var created = pushEvent.FirstOrDefault().CreatedAt;
                    commits.AddRange(pushEvent
                        .SelectMany(c => c.Commits)
                        .Select(c => new GitCommitModel
                        {
                            Id = c.sha,
                            RepoName = pushEvent.Key,
                            Comment = c.message,
                            By = c.author?.name,
                            Date = created
                        }));
                }

                return new GitCommitResult
                {
                    data = commits.ToArray()
                };
            }
            catch
            {
                return new GitCommitResult
                {
                    errors = new[] { "Unable to get GitHub data" }
                };
            }
        }
    }

    public interface IGetGitHubCommitsQuery
    {
        Task<GitCommitResult> Execute(GetGitHubCommits request);
    }
}
