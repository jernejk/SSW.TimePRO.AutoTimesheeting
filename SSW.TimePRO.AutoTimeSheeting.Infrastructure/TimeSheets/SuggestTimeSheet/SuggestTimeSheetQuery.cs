using SSW.TimePRO.AutoTimeSheeting.Infrastructure.AzureDevOps;
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
                DateCreated = date,
                SuggestedActions = new SuggestedActions()
            };

            if (appointment == null || appointment.clientId == "SSW")
            {
                result.ClientID = "SSW";
                result.CategoryID = GetLeaveCategoryId(appointment?.title);
                result.ProjectID = result.CategoryID != null ? "LEAVE" : null;

                if (result.ProjectID == null)
                {
                    string repoName = request.Commits?
                        .Where(c => !string.IsNullOrEmpty(c.RepoName))
                        .Select(c => c.RepoName)
                        .FirstOrDefault();

                    // SSW.Bots.TimeSheets, SSW.Sophie.Web
                    repoName = repoName?.ToLowerInvariant() ?? string.Empty;
                    if (repoName.Equals("SSW.TimeProDotNet", StringComparison.OrdinalIgnoreCase)
                        || repoName.Equals("SSW.Bots.TimeSheets", StringComparison.OrdinalIgnoreCase))
                    {
                        result.ProjectID = "TP";
                    }
                    else if (repoName.IndexOf("bot", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        result.ProjectID = "8897DK";
                    }
                    else if (repoName.IndexOf("Sophie", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        result.ProjectID = "GVOUF1";
                    }

                    // Default to SophieHub for now.
                    if (string.IsNullOrWhiteSpace(result.ProjectID))
                    {
                        result.SuggestedActions.SelectProject = true;
                        result.ProjectID = "GVOUF1";
                    }

                    result.CategoryID = "WEBDEV";
                }
            }
            else
            {
                // If there are more than 2 projects for the same client, suggested action is to select a project.
                result.SuggestedActions.SelectProject = request.RecentProjects
                    .Count(a => a.ClientID.Equals(appointment?.clientId, StringComparison.OrdinalIgnoreCase)) > 1;
            }

            result.Comment = GenerateComment(result, appointment, request.Commits);
            result.AlreadyHasTimesheet = request?.Timesheets?.Any() == true;

            if (string.IsNullOrWhiteSpace(result.Comment))
            {
                result.SuggestedActions.EnterDescription = true;
            }

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
