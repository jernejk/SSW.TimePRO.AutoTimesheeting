﻿using Flurl;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.CreateTimeSheet
{
    public class CreateTimeSheetCommand : ICreateTimeSheetCommand
    {
        public async Task<CreateTimeSheetModel> Execute(CreateTimeSheetRequest request)
        {
            var date = DateTime.Parse(request.DateCreated);
            var url = new Url(request.TenantUrl)
                .AppendPathSegment($"/Timesheet/{request.EmpID}/{date.ToString("yyyy-MM-dd")}/Add")
                // TimePro bug: Basic auth is not base64 decoded on the server and takes the raw token.
                .WithHeader("Authorization", $"Basic {request.Token}")
                .WithHeader("Content-Type", "application/x-www-form-urlencoded");

            var result = new CreateTimeSheetModel();

            try
            {
                string content = await GenerateBody(request, date);
                var response = await url.PostStringAsync(content);

                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    result.IsSuccessful = true;
                }
            }
            catch (Exception e)
            {
            }

            return result;
        }

        private static async Task<string> GenerateBody(CreateTimeSheetRequest request, DateTime date)
        {
            var form = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("EmpTime.TimeID", "0"),
                    new KeyValuePair<string, string>("EmpTime.IsAutomaticComments", "False"),
                    new KeyValuePair<string, string>("EmpTime.EmpID", request.EmpID),
                    new KeyValuePair<string, string>("EmpTime.TfsComments", ""),
                    new KeyValuePair<string, string>("EmpTime.DateCreated", date.ToString("dd/MM/yyyy")),
                    new KeyValuePair<string, string>("EmpTime.ClientID", request.ClientID),
                    new KeyValuePair<string, string>("EmpTime.ProjectID", request.ProjectID),
                    new KeyValuePair<string, string>("UseIteration", "false"),
                    new KeyValuePair<string, string>("EmpTime.CategoryID", request.CategoryID),
                    new KeyValuePair<string, string>("EmpTime.LocationID", request.LocationID)
                });

            // Due different encodings, it's FormUrlEncodedContent no longer is helpful.
            var content = await form.ReadAsStringAsync();
            content += $"&EmpTime.TimeStart=8%3A00+AM&EmpTime.TimeEnd=5%3A00+PM&EmpTime.TimeLess=1&TotalTimeTextBox=8";
            content += $"&EmpTime.BillableID={request.BillableID}&EmpTime.IsBillingTypeOverridden=false";
            content += $"&EmpTime.SellPrice={request.ClientEmpRate}&EmpTime.SalesTaxPct={request.ClientTaxRate}&EmpCreated=&EmpUpdated=&SaveType=Add";
            content += "&Note=" + WebUtility.UrlEncode(request.Comment);
            return content;
        }
    }

    public interface ICreateTimeSheetCommand
    {
        Task<CreateTimeSheetModel> Execute(CreateTimeSheetRequest request);
    }
}
