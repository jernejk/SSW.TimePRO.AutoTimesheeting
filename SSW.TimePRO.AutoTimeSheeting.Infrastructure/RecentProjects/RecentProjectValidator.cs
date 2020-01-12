using FluentValidation;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.RecentProjects
{
    public class RecentProjectValidator : AbstractValidator<GetRecentProjects>
    {
        public RecentProjectValidator()
        {
            RuleFor(m => m.EmpID).ForEmpID();
            RuleFor(m => m.TenantUrl).ForTenantUrl();
            RuleFor(m => m.Token).ForToken();
        }
    }
}
