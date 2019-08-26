using System;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.GitHub
{
    public class GetGitHubCommitsRequest
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public string Date { get; set; }
    }
}
