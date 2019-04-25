using System;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.Crm
{
    public class GetCrmAppointmentsRequest : BaseTimeProRequest
    {
        public GetCrmAppointmentsRequest(string tenantUrl, string empID, DateTime startUtc, DateTime endUtc, string token)
            : base(tenantUrl, token)
        {
            EmpID = empID;
            StartUtc = startUtc;
            EndUtc = endUtc;
        }

        public string EmpID { get; }
        public DateTime StartUtc { get; }
        public DateTime EndUtc { get; }
    }
}
