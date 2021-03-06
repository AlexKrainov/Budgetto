using System.Threading;
using System.Threading.Tasks;

namespace Scheduler.Service
{
    public interface IHostedService
    {
        //
        // Summary:
        //     Triggered when the application host is ready to start the service.
        Task StartAsync(CancellationToken cancellationToken);
        //
        // Summary:
        //     Triggered when the application host is performing a graceful shutdown.
        Task StopAsync(CancellationToken cancellationToken);
    }
}
