using dotenv.net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SampleWebJob
{
    class Program
    {
        static async Task Main(string[] args)
        {

            Activity.DefaultIdFormat = ActivityIdFormat.W3C; //Configure correlated log id's
            DotEnv.Config(false); // Allow use of a .env file in dev, replaced with actual env vars in a container/azure on deployment

            IHost host = new HostBuilder()
             .ConfigureHostConfiguration(configHost =>
             {
                 configHost.SetBasePath(Directory.GetCurrentDirectory());
                 configHost.AddEnvironmentVariables(prefix: "APPNAME_");
                 configHost.AddCommandLine(args);
             })
             .ConfigureAppConfiguration((hostContext, configApp) =>
             {
                 configApp.SetBasePath(Directory.GetCurrentDirectory());
                 configApp.AddJsonFile($"appsettings.json", true);
                 configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true);
                 configApp.AddUserSecrets("AppName", true); // allow devs to use a user secrets json file for privileged config.
                 configApp.AddEnvironmentVariables(prefix: "APPNAME_");
                 configApp.AddCommandLine(args);
             })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddLogging();
                services.AddHostedService<ApplicationHostService>(); //TODO: Add your custom processing services here.
            })
            .ConfigureLogging((hostContext, configLogging) =>
            {
                configLogging.AddSerilog(new LoggerConfiguration()
                          .ReadFrom.Configuration(hostContext.Configuration)
                          .CreateLogger());

                Log.Information("Environment: {environment}", hostContext.HostingEnvironment.EnvironmentName);
            })
            .Build();

            try
            {
                await host.RunAsync().ConfigureAwait(true);
            }
            catch (HostingStopException)
            {
                //Host terminated
            }
        }
    }
}
