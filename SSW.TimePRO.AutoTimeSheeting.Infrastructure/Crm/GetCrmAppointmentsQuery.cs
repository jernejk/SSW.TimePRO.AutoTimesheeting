using Flurl;
using Flurl.Http;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.Crm
{
    public class GetCrmAppointmentsQuery : IGetCrmAppointmentsQuery
    {
        public Task<CrmAppointmentModel[]> Execute(GetCrmAppointmentsRequest request)
        {
            DateTimeOffset startDate = DateTimeOffset.Parse(request.Start, CultureInfo.InvariantCulture);
            DateTimeOffset endDate = DateTimeOffset.Parse(request.End, CultureInfo.InvariantCulture);

            if (request.Start == request.End)
            {
                endDate = endDate.AddDays(1);
            }

            var url = new Url(request.TenantUrl)
                .AppendPathSegment("/Crm/Appointments")
                .SetQueryParam("employeeID", request.EmpID)
                .SetQueryParam("start", startDate.ToUnixTimeSeconds())
                .SetQueryParam("end", endDate.ToUnixTimeSeconds())
                .WithBasicAuth(request.Token, string.Empty);

            return url.GetJsonAsync<CrmAppointmentModel[]>();
        }
    }

    public interface IGetCrmAppointmentsQuery
    {
        Task<CrmAppointmentModel[]> Execute(GetCrmAppointmentsRequest request);
    }
}
