// Copyright (c) Timofei Zhakov. All rights reserved.

using Haukcode.HighResolutionTimer;

namespace MrBoom.Server
{
    public abstract class TimerService : BackgroundService
    {
        private readonly Haukcode.HighResolutionTimer.ITimer timer;

        public TimerService(int periodMS)
        {
            timer = new HighResolutionTimer();
            timer.SetPeriod(periodMS);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            timer.Start();
            await Task.Run(() => ExecuteSync(stoppingToken));
        }

        private void ExecuteSync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                timer.WaitForTrigger();
                _ = TickAsync(stoppingToken);
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
