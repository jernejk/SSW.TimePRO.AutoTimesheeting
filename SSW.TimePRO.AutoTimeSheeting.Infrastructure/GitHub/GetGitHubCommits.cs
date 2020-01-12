using MediatR;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.AzureDevOps;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.GitHub
{
    public class GetGitHubCommits : IRequest<GitCommitResult>
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public string Date { get; set; }
    }
}
