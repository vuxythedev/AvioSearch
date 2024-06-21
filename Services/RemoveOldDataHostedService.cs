using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ServiceContracts;

namespace Services
{
    public class RemoveOldDataHostedService : IHostedService
    {
        private Timer? _timer;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly HostedServiceOptions _hostedServiceConfigOptions;


        public RemoveOldDataHostedService(IServiceScopeFactory flightSearchService, IOptions<HostedServiceOptions> hostedServiceConfigOptions)
        {
            _serviceScopeFactory = flightSearchService;
            _hostedServiceConfigOptions = hostedServiceConfigOptions.Value;
        }
           

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            int refreshTime = int.Parse(_hostedServiceConfigOptions.RefreshTime!);
                
            _timer = new Timer(async state => await DeleteFlightSearchDataAsync(state), null, TimeSpan.Zero, TimeSpan.FromMinutes(refreshTime));

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
           
            return Task.CompletedTask;
            // graceful shoutdown!!
            //catch sigint
        }
       

        private async Task DeleteFlightSearchDataAsync(object? state)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var flightSearchService = scope.ServiceProvider.GetRequiredService<IFlightSearchService>();
                int pruneDataPeriod = int.Parse(_hostedServiceConfigOptions.PruneDataPeriod!);             
                await flightSearchService.DeleteFlightSearchDataAsync(pruneDataPeriod);
            }         
        }


        public class HostedServiceOptions
        {
            public string? PruneDataPeriod { get; set; }
            public string? RefreshTime { get; set; }           
        }
     
    }
}
