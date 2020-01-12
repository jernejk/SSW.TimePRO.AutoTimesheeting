using FluentValidation;
using MediatR;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SSW.TimePRO.Automation.AzureFunctions;
using SSW.TimePRO.Automation.AzureFunctions.Behaviors;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.Crm;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.GitHub;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.RecentProjects;

[assembly: FunctionsStartup(typeof(Startup))]
namespace SSW.TimePRO.Automation.AzureFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddMediatR(typeof(GetRecentProjectsHandler));
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            builder.Services.AddSingleton<IValidator<GetRecentProjects>, RecentProjectValidator>();
            builder.Services.AddSingleton<IValidator<GetCrmAppointments>, GetCrmAppointmentsValidator>();
            builder.Services.AddSingleton<IValidator<GetGitHubCommits>, GetGitHubCommitsValidator>();
        }
    }
}
