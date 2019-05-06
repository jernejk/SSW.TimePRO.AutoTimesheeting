using Autofac;
using AzureFunctions.Autofac.Configuration;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.ClientRate;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.Crm;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.RecentProjects;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.CollectData;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.CreateTimeSheet;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.SuggestTimeSheet;

namespace SSW.TimePRO.AzureFunctions
{
    public class DIConfig
    {
        public DIConfig(string functionName)
        {
            DependencyInjection.Initialize(builder =>
            {
                builder.RegisterType<GetRecentProjectsQuery>().As<IGetRecentProjectsQuery>();
                builder.RegisterType<GetClientRateQuery>().As<IGetClientRateQuery>();
                builder.RegisterType<GetCrmAppointmentsQuery>().As<IGetCrmAppointmentsQuery>();
                builder.RegisterType<CollectDataQuery>().As<ICollectDataQuery>();
                builder.RegisterType<SuggestTimeSheetQuery>().As<ISuggestTimeSheetQuery>();
                builder.RegisterType<CreateTimeSheetCommand>().As<ICreateTimeSheetCommand>();
            }, functionName);
        }
    }
}
