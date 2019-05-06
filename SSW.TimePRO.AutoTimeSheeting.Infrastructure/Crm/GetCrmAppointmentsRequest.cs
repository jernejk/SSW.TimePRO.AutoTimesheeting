using System;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.Crm
{
    public class GetCrmAppointmentsRequest : BaseTimeProRequest
    {
        public GetCrmAppointmentsRequest(string tenantUrl, string empID, string start, string end, string token)
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
