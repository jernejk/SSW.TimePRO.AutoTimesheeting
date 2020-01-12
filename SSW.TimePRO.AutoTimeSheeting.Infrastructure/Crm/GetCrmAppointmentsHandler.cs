using Flurl;
using Flurl.Http;
using MediatR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.Crm
{
    public class GetCrmAppointmentsHandler : IGetCrmAppointmentsQuery, IRequestHandler<GetCrmAppointments, IEnumerable<CrmAppointmentModel>>
    {

        public Task<IEnumerable<CrmAppointmentModel>> Execute(GetCrmAppointments request)
        {
            return Handle(request, default);
        }

        public async Task<IEnumerable<CrmAppointmentModel>> Handle(GetCrmAppointments request, CancellationToken cancellationToken)
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
                // TimePro bug: Basic auth is not base64 decoded on the server and takes the raw token.
                .WithHeader("Authorization", $"Basic {request.Token}");

            return await url.GetJsonAsync<CrmAppointmentModel[]>();
        }
    }

    public interface IGetCrmAppointmentsQuery
    {
        Task<IEnumerable<CrmAppointmentModel>> Execute(GetCrmAppointments request);
    }
}
