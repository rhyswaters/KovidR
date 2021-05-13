using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CaseNumbersWorker.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CaseNumbersWorker
{
    public class Worker : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<Worker> _logger;
        private Timer _timer;

        public Worker(ILogger<Worker> logger, IServiceProvider services)
        {
            _logger = logger;
            Services = services;
        }

        public IServiceProvider Services { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            TimeSpan interval = TimeSpan.FromHours(24);

            //always run immediately on startup no matter what time of day before scheduling to run at 2pm tomorrow and every day thereafter
            DoWork(null);

            var nextRunTime = DateTime.Today.AddDays(1).AddHours(14);
            var curTime = DateTime.Now;
            var firstInterval = nextRunTime.Subtract(curTime);

            Action action = () =>
            {
                var t1 = Task.Delay(firstInterval);
                t1.Wait();
                DoWork(null);

                _timer = new Timer(
                    DoWork,
                    null,
                    TimeSpan.Zero,
                    interval
                );
            };

            Task.Run(action);
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            var count = Interlocked.Increment(ref executionCount);

            using (var scope = Services.CreateScope())
            {
                var caseNumbersService =
                    scope.ServiceProvider
                        .GetRequiredService<ICaseNumbersService>();

                await caseNumbersService.PublishCaseNumbers();
            }

            _logger.LogInformation(
                "Timed Hosted Service is working. Count: {Count}", count);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
