using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using SampleCollector.Interfaces;

namespace SampleCollector.Services
{
    public class MessagesReceiverService : BackgroundService
    {
        private readonly ISampleCollector _collector;
        private readonly string[] names = { "test", "test_1", "test_2", "test_33", "tony" };

        public MessagesReceiverService(ISampleCollector sampleCollector)
        {
            _collector = sampleCollector;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Parallel.For(0, 2, (i) =>
                {
                    ReceiveMessage();
                });

                await Task.Delay(2000);
            }
        }

        private async Task ReceiveMessage()
        {
            var dataVariations = new string[] { "<menu> sdaksodkad </menu>", "{\"name\": \"test_123123\"}", "this is a csv file " };

            foreach (var item in names)
            {
                Random random = new Random();
                BinaryData content = new BinaryData(dataVariations[random.Next(0, dataVariations.Length)]);
                await _collector.ProcessMessageAsync(item, content);
            }
        }
    }
}