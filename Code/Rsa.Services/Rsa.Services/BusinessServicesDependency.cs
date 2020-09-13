using Microsoft.Extensions.DependencyInjection;
using Rsa.Services.Abstractions;
using Rsa.Services.Implementations;
using System;

namespace Rsa.Services
{
    public static class BusinessServicesDependency
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddTransient<IReportActivities, ReportActivities>();
            return services;
        }
    }
}
