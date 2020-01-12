using FluentValidation;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.Crm
{
    public class GetCrmAppointmentsValidator : AbstractValidator<GetCrmAppointments>
    {
        public GetCrmAppointmentsValidator()
        {
            RuleFor(m => m.EmpID).ForEmpID();
            RuleFor(m => m.TenantUrl).ForTenantUrl();
            RuleFor(m => m.Token).ForToken();
            RuleFor(m => m.Start).ForDate("Start date");
            RuleFor(m => m.End).ForDate("End date");
        }
    }
}
