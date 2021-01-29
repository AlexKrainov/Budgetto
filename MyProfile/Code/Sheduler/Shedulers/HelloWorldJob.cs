using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading.Tasks;

namespace MyProfile.Code.Sheduler.Shedulers
{
    /// <summary>
    /// https://andrewlock.net/creating-a-quartz-net-hosted-service-with-asp-net-core/
    /// </summary>
    public class HelloWorldJob : IJob
    {
        private readonly ILogger<HelloWorldJob> _logger;

        public HelloWorldJob(ILogger<HelloWorldJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Hello world!");
            return Task.CompletedTask;
        }
    }
}
