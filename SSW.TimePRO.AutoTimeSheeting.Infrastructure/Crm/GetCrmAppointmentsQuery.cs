using Flurl;
using Flurl.Http;
using System;
using System.Threading.Tasks;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.Crm
{
    public class GetCrmAppointmentsQuery : IGetCrmAppointmentsQuery
    {
        public Task<CrmAppointmentModel[]> Execute(GetCrmAppointmentsRequest request)
        {
            if (request.StartUtc.Kind != DateTimeKind.Utc || request.EndUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Dates needs to be in UTC", nameof(request));
            }

            var url = new Url(request.TenantUrl)
                .AppendPathSegment("/Crm/Appointments")
                .SetQueryParam("employeeID", request.EmpID)
                .SetQueryParam("start", ((DateTimeOffset)request.StartUtc).ToUnixTimeSeconds())
                .SetQueryParam("end", ((DateTimeOffset)request.EndUtc).ToUnixTimeSeconds())
                .WithBasicAuth(request.Token, string.Empty);

            return url.GetJsonAsync<CrmAppointmentModel[]>();
        }
    }

    public interface IGetCrmAppointmentsQuery
    {
        Task<CrmAppointmentModel[]> Execute(GetCrmAppointmentsRequest request);
    }
}
