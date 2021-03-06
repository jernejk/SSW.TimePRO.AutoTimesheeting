﻿using Flurl;
using Flurl.Http;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.AzureDevOps
{
    public class GetCommitsByEmpIDHandler : IGetCommitsByEmpIDQuery, IRequestHandler<GetCommitsByEmpID, IEnumerable<GitCommitModel>>
    {
        public Task<IEnumerable<GitCommitModel>> Execute(GetCommitsByEmpID request)
        {
            return Handle(request, default);
        }

        public async Task<IEnumerable<GitCommitModel>> Handle(GetCommitsByEmpID request, CancellationToken cancellationToken)
        {
            var url = new Url(request.TenantUrl)
                .AppendPathSegment("/Ajax/GetTfsSubscriptions")
                .SetQueryParam("employeeID", request.EmpID)
                // TimePro bug: Basic auth is not base64 decoded on the server and takes the raw token.
                .WithHeader("Authorization", $"Basic {request.Token}");

            var subs = await url.GetJsonAsync<AzureDevOpsSubscriptionResult>();

            // SSW Azure DevOps are not supported ATM.
            var subscriptions = subs.data;

            var commits = await Task.WhenAll(subscriptions.Select(s => GetCommits(request, s)));
            return commits.SelectMany(c => c);
        }

        private async Task<IEnumerable<GitCommitModel>> GetCommits(GetCommitsByEmpID request, AzureDevOpsSubscription subscription)
        {
            DateTimeOffset startDate = DateTimeOffset.Parse(request.Date, CultureInfo.InvariantCulture);
            DateTimeOffset endDate = startDate.AddDays(1);

            string content =
                $"------WebKitFormBoundary7MA4YWxkTrZu0gW\r\n" +
                $"Content-Disposition: form-data; name=\"employeeID\"\r\n\r\n{request.EmpID}\r\n" +
                $"------WebKitFormBoundary7MA4YWxkTrZu0gW\r\n" +
                $"Content-Disposition: form-data; name=\"startDate\"\r\n\r\n{startDate.ToString("yyyy-MM-dd")}\r\n" +
                $"------WebKitFormBoundary7MA4YWxkTrZu0gW\r\n" +
                $"Content-Disposition: form-data; name=\"endDate\"\r\n\r\n{endDate.ToString("yyyy-MM-dd")}\r\n" +
                $"------WebKitFormBoundary7MA4YWxkTrZu0gW\r\n" +
                $"Content-Disposition: form-data; name=\"settings[Id]\"\r\n\r\n{subscription.Id}\r\n" +
                $"------WebKitFormBoundary7MA4YWxkTrZu0gW--";

            var url = new Url(request.TenantUrl)
                .AppendPathSegment("/Ajax/GetTfsSubChangeHistory")
                .WithHeader("content-type", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW")
                // TimePro bug: Basic auth is not base64 decoded on the server and takes the raw token.
                .WithHeader("Authorization", $"Basic {request.Token}");

            var response = await url.PostStringAsync(content);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<GitCommitResult>(json);

            return result.data;
        }
    }

    public interface IGetCommitsByEmpIDQuery
    {
        Task<IEnumerable<GitCommitModel>> Execute(GetCommitsByEmpID request);
    }
}
