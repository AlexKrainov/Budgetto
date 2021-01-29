using MyProfile.UserLog.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Scheduler.Service.Test
{
    public class MyCronJob1 : CronJobService
    {
        private UserLogService userLogService;

        public MyCronJob1(IScheduleConfig<MyCronJob1> config, UserLogService userLogService )
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            this.userLogService = userLogService;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
           // _logger.LogInformation("CronJob 1 starts.");
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            // _logger.LogInformation($"{DateTime.Now:hh:mm:ss} CronJob 1 is working.");
            userLogService.CreateErrorLog(Guid.Parse("4C88F103-9306-489F-A196-FFCDEBC03E9E"), "DoWork", null);
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            //_logger.LogInformation("CronJob 1 is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}
