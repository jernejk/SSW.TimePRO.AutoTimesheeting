using FluentValidation;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.GitHub
{
    public class GetGitHubCommitsValidator : AbstractValidator<GetGitHubCommits>
    {
        public GetGitHubCommitsValidator()
        {
            RuleFor(m => m.Date).ForDate("Date");
            RuleFor(m => m.Username).ForUsername();
            RuleFor(m => m.Token).ForToken();
        }
    }
}
