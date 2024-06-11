using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ServiceContracts;

namespace Services
{
    public class HostedService : IHostedService
    {
        private Timer? _timer;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly HostedServiceOptions _hostedServiceConfigOptions;


        public HostedService(IServiceScopeFactory flightSearchService, IOptions<HostedServiceOptions> hostedServiceConfigOptions)
        {
            _serviceScopeFactory = flightSearchService;
            _hostedServiceConfigOptions = hostedServiceConfigOptions.Value;
        }
           

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            int pruneDataPeriod = int.Parse(_hostedServiceConfigOptions.PruneDataPeriod!);

            _timer = new Timer(callback: DeleteFlightSearchData, null, dueTime: TimeSpan.Zero, period: TimeSpan.FromMinutes(pruneDataPeriod));
           
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
           
            return Task.CompletedTask;
            // graceful shoutdown!!
            //catch sigint
        }
       

        private void DeleteFlightSearchData(object? state)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var flightSearchService = scope.ServiceProvider.GetRequiredService<IFlightSearchService>();

                int refreshTime = int.Parse( _hostedServiceConfigOptions.RefreshTime!);
                flightSearchService.DeleteFlightSearchDataAsync(refreshTime);
            }         
        }


        public class HostedServiceOptions
        {
            public string? PruneDataPeriod { get; set; }
            public string? RefreshTime { get; set; }           
        }

    }
}
