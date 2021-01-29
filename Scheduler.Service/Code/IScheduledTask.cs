using System.Threading;
using System.Threading.Tasks;

namespace Scheduler.Service
{
    public interface IScheduledTask
    {
        string Schedule { get; }
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
