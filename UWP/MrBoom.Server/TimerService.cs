// Copyright (c) Timofei Zhakov. All rights reserved.

using Haukcode.HighResolutionTimer;

namespace MrBoom.Server
{
    public abstract class TimerService : BackgroundService
    {
        private readonly Haukcode.HighResolutionTimer.ITimer timer;
        private readonly ILogger logger;

        public TimerService(int periodMS, ILogger logger)
        {
            timer = new HighResolutionTimer();
            timer.SetPeriod(periodMS);

            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            timer.Start();
            await Task.Run(() => ExecuteSync(stoppingToken), stoppingToken);
        }

        private void ExecuteSync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                timer.WaitForTrigger();

                Task.Run(async () =>
                {
                    try
                    {
                        await TickAsync(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Unhandled exception occurred in the handler");
                    }
                }, stoppingToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Stop();
            return Task.CompletedTask;
        }

        protected abstract Task TickAsync(CancellationToken stoppingToken);
    }
}
