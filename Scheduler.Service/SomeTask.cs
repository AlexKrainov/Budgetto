using System;
using System.Threading;
using System.Threading.Tasks;

namespace Scheduler.Service
{
    public class SomeTask : IScheduledTask
    {
        //https://blog.maartenballiauw.be/post/2017/08/01/building-a-scheduled-cache-updater-in-aspnet-core-2.html
        public string Schedule => "1 * * * *";

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            // do stuff
        }
    }
}
