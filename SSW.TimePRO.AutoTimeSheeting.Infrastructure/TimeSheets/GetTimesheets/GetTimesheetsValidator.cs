using FluentValidation;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.GetTimesheets
{
    public class GetTimesheetsValidator : AbstractValidator<GetTimesheets>
    {
        public GetTimesheetsValidator()
        {
            RuleFor(m => m.EmpID).ForEmpID();
            RuleFor(m => m.TenantUrl).ForTenantUrl();
            RuleFor(m => m.Token).ForToken();
            RuleFor(m => m.Start).ForDate("Start date");
            RuleFor(m => m.End).ForDate("End date");
        }
    }
}
