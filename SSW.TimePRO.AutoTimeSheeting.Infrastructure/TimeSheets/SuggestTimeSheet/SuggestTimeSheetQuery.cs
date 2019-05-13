﻿using SSW.TimePRO.AutoTimeSheeting.Infrastructure.AzureDevOps;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.Crm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.SuggestTimeSheet
{
    public class SuggestTimeSheetQuery : ISuggestTimeSheetQuery
    {
        public Task<SuggestTimeSheetModel> Execute(SuggestTimeSheetRequest request)
        {
            string date = request.Date;
            if (date.Contains("+"))
            {
                date = date.Substring(0, date.IndexOf("+"));
            }

            var appointment = request.CrmAppointments.FirstOrDefault(a => a.start.StartsWith(date));
            var project = request.RecentProjects.FirstOrDefault(a => a.ClientID.Equals(appointment?.clientId, StringComparison.OrdinalIgnoreCase));
            var result = new SuggestTimeSheetModel
            {
                EmpID = request.EmpID,
                ClientID = appointment?.clientId,
                ProjectID = project?.ProjectID,
                CategoryID = project?.CategoryID,
                BillableID = project?.ClientID != null && project?.ClientID.Equals("SSW") != true ? "B" : "W",
                LocationID = "SSW",
                DateCreated = date
            };

            if (appointment == null || appointment.clientId == "SSW")
            {
                result.CategoryID = GetLeaveCategoryId(appointment?.title);
                result.ProjectID = result.CategoryID != null ? "LEAVE" : null;
            }

            result.Comment = GenerateComment(result, appointment, request.Commits);
            result.AlreadyHasTimesheet = request?.Timesheets?.Any() == true;

            return Task.FromResult(result);
        }

        private string GetLeaveCategoryId(string subject)
        {
            switch (subject)
            {
                case string _ when subject.IndexOf("sick", StringComparison.OrdinalIgnoreCase) >= 0:
                    return "LSICK";

                case string _ when subject.IndexOf("public", StringComparison.OrdinalIgnoreCase) >= 0:
                    return "LNWD";

                case string _ when subject.IndexOf("day", StringComparison.OrdinalIgnoreCase) >= 0:
                    return "L-ANN";
            }

            return null;
        }

        private string GenerateComment(SuggestTimeSheetModel model, CrmAppointmentModel appointment, IEnumerable<GitCommitModel> commits)
        {
            if (model.ClientID == "SSW" && model.ProjectID == "LEAVE")
            {
                switch (model.CategoryID)
                {
                    case "LSICK":
                        return "Sick leave";

                    case "LNWD":
                        return appointment?.title;

                    case "L-ANN":
                        return "Annual leave";
                }
            }
            else if (commits?.Any() == true)
            {
                var messages = commits
                    .Where(c => c.Comment.IndexOf("merge", StringComparison.OrdinalIgnoreCase) != 0)
                    .Select(c => c.Comment)
                    .ToList();

                if (messages.Any())
                {
                    return $"Commits:\n- {string.Join("\n- ", messages)}";
                }
            }

            return null;
        }
    }

    public interface ISuggestTimeSheetQuery
    {
        Task<SuggestTimeSheetModel> Execute(SuggestTimeSheetRequest request);
    }
}