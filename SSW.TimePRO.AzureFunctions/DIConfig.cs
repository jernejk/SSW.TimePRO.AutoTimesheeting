using Autofac;
using AzureFunctions.Autofac.Configuration;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.ClientRate;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.Crm;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.RecentProjects;

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
            }, functionName);
        }
    }
}
