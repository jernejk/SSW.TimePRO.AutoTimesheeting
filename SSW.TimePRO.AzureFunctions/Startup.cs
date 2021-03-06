﻿using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.AzureDevOps;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.ClientRate;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.Crm;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.GitHub;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.RecentProjects;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.CollectData;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.CreateTimeSheet;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.GetTimesheets;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.SuggestTimeSheet;
using SSW.TimePRO.AzureFunctions;

[assembly: FunctionsStartup(typeof(Startup))]
namespace SSW.TimePRO.AzureFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // register some services
            builder.Services.AddTransient<IGetRecentProjectsQuery, GetRecentProjectsHandler>();
            builder.Services.AddTransient<IGetClientRateQuery, GetClientRateQuery>();
            builder.Services.AddTransient<IGetCrmAppointmentsQuery, GetCrmAppointmentsHandler>();
            builder.Services.AddTransient<ICollectDataQuery, CollectDataQuery>();
            builder.Services.AddTransient<ISuggestTimeSheetQuery, SuggestTimeSheetQuery>();
            builder.Services.AddTransient<ICreateTimeSheetCommand, CreateTimeSheetCommand>();
            builder.Services.AddTransient<IGetCommitsByEmpIDQuery, GetCommitsByEmpIDHandler>();
            builder.Services.AddTransient<IGetTimesheetsQuery, GetTimesheetsHandler>();
            builder.Services.AddTransient<IGetGitHubCommitsQuery, GetGitHubCommitsHandler>();
        }
    }
}
