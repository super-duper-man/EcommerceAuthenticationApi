using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Infrastructure.Data;
using AuthenticationApi.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Resource.Share.Lib.DependencyInjection;

namespace AuthenticationApi.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            ShareServiceContainer.AddShareService<AuthenticationDbContext>(services, config, config["AppSerilog:FileName"]!);

            services.AddScoped<IUser, UserRepository>();

            return services;
        }

        public static IApplicationBuilder UserInfrastructurePolicy(this IApplicationBuilder app)
        {
            ShareServiceContainer.UseSharedPolicies(app);

            return app;
        }
    }
}
