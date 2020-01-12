using MediatR;
using System.Collections.Generic;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.Crm
{
    public class GetCrmAppointments : BaseTimeProRequest, IRequest<IEnumerable<CrmAppointmentModel>>
    {
        public GetCrmAppointments(string tenantUrl, string empID, string start, string end, string token)
            : base(tenantUrl, token)
        {
            EmpID = empID;
            Start = start.Replace(" ", "+");
            End = end.Replace(" ", "+");
        }

        public string EmpID { get; }
        public string Start { get; }
        public string End { get; }
    }
}
