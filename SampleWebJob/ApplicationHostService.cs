using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace SampleWebJob
{
    public class ApplicationHostService : IHostedService
    {
        readonly ILogger<ApplicationHostService> _logger;
        readonly IConfiguration _configuration;
        readonly IHostEnvironment _hostingEnvironment;
        public ApplicationHostService(
            ILogger<ApplicationHostService> logger,
            IConfiguration configuration,
            IHostEnvironment hostingEnvironment
            )
        {
            _logger = logger;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask.ConfigureAwait(false);

            //Do something
            _logger.LogWarning("Hello from console application");

            //Throw exception to terminate the host
            throw new HostingStopException();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
