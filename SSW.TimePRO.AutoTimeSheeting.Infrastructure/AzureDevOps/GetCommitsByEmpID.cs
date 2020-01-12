using MediatR;
using System.Collections.Generic;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.AzureDevOps
{
    public class GetCommitsByEmpID : BaseTimeProRequest, IRequest<IEnumerable<GitCommitModel>>
    {
        public GetCommitsByEmpID(string tenantUrl, string empId, string date, string token)
            : base(tenantUrl, token)
        {
            EmpID = empId;
            Date = date;
        }

        public string EmpID { get; set; }
        public string Date { get; set; }
    }
}
