using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleHostServiceExample
{
    public class FakedHostService : IHostedService
    {
        private readonly IHostApplicationLifetime _applicationLifetime;
        private IDisposable _tokenSource;
        public FakedHostService(IHostApplicationLifetime applicationLifetime)
        {
            _applicationLifetime = applicationLifetime;
            _applicationLifetime.ApplicationStarted.Register(() =>
                Console.WriteLine($"[{0}]app started", DateTimeOffset.Now)
            );

            _applicationLifetime.ApplicationStopping.Register(() =>
                Console.WriteLine($"[{0}]app is stopping", DateTimeOffset.Now)
            );

            _applicationLifetime.ApplicationStopped.Register(() =>
                Console.WriteLine($"[{0}]app closed", DateTimeOffset.Now)
            );
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5))
                .Token.Register(_applicationLifetime.StopApplication);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            
            return Task.CompletedTask;
        }
    }
}
