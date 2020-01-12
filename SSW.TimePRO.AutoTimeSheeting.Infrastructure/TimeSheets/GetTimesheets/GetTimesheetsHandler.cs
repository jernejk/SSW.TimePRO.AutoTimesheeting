using Flurl;
using Flurl.Http;
using MediatR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.GetTimesheets
{
    public class GetTimesheetsHandler : IGetTimesheetsQuery, IRequestHandler<GetTimesheets, IEnumerable<TimesheetModel>>
    {
        public Task<IEnumerable<TimesheetModel>> Execute(GetTimesheets request)
        {
            return Handle(request, default);
        }

        public async Task<IEnumerable<TimesheetModel>> Handle(GetTimesheets request, CancellationToken cancellationToken)
        {
            DateTimeOffset startDate = DateTimeOffset.Parse(request.Start, CultureInfo.InvariantCulture);
            DateTimeOffset endDate = DateTimeOffset.Parse(request.End, CultureInfo.InvariantCulture);

            if (request.Start == request.End)
            {
                endDate = endDate.AddDays(1).AddMinutes(-1);
            }

            var url = new Url(request.TenantUrl)
                .AppendPathSegment("/Ajax/GetTimesheets")
                .SetQueryParam("employeeID", request.EmpID)
                .SetQueryParam("start", startDate.ToUnixTimeSeconds())
                .SetQueryParam("end", endDate.ToUnixTimeSeconds())
                // TimePro bug: Basic auth is not base64 decoded on the server and takes the raw token.
                .WithHeader("Authorization", $"Basic {request.Token}");

            return await url.GetJsonAsync<TimesheetModel[]>();
        }
    }

    public interface IGetTimesheetsQuery
    {
        Task<IEnumerable<TimesheetModel>> Execute(GetTimesheets request);
    }
}
