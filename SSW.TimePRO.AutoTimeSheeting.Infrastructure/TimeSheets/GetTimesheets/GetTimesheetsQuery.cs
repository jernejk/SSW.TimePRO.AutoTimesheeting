using Flurl;
using Flurl.Http;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.GetTimesheets
{
    public class GetTimesheetsQuery : IGetTimesheetsQuery
    {
        public Task<TimesheetModel[]> Execute(GetTimesheetsRequest request)
        {
            DateTimeOffset startDate = DateTimeOffset.Parse(request.Start, CultureInfo.InvariantCulture);
            DateTimeOffset endDate = DateTimeOffset.Parse(request.End, CultureInfo.InvariantCulture);

            if (request.Start == request.End)
            {
                endDate = endDate.AddDays(1);
            }

            var url = new Url(request.TenantUrl)
                .AppendPathSegment("/Ajax/GetTimesheets")
                .SetQueryParam("employeeID", request.EmpID)
                .SetQueryParam("start", startDate.ToUnixTimeSeconds())
                .SetQueryParam("end", endDate.ToUnixTimeSeconds())
                .WithBasicAuth(request.Token, string.Empty);

            return url.GetJsonAsync<TimesheetModel[]>();
        }
    }

    public interface IGetTimesheetsQuery
    {
        Task<TimesheetModel[]> Execute(GetTimesheetsRequest request);
    }
}
