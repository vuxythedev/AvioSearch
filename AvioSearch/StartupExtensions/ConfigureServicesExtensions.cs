using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;
using RepositoryContracts;
using ServiceContracts;
using Services;
using static Services.RemoveOldDataHostedService;

namespace AvioSearch.StartupExtensions
{
    public static class ConfigureServicesExtensions
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllersWithViews();
            services.AddDbContext<FlightDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.Configure<AmadeusConfigOptions>(configuration.GetSection("AmadeusAccessData"));
            services.Configure<HostedServiceOptions>(configuration.GetSection("HostedService"));
            services.AddHostedService<RemoveOldDataHostedService>();

            #region IOC

            services.AddScoped<IProviderService, AmadeusService>();
            services.AddScoped<IFlightSearchRepository, FlightSearchRepository>();
            services.AddScoped<IFlightSearchService, FlightSearchService>();

            #endregion


            return services;
        }
    }
}
