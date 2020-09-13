using Microsoft.Extensions.DependencyInjection;
using Rsa.Repository.Abstractions;
using Rsa.Repository.Implementation;

namespace Rsa.Repository
{
    public static class RepositoryDependency
    {
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services) 
        {
            services.AddTransient(typeof(IRepository), typeof(Repository));
            services.AddTransient<IUnitOfWork,UnitOfWork>();
            return services;
        }
    }
}
