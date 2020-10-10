using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace client.kafka.consumer.core
{
    public class KafkaConsumerHostedService : IHostedService
    {
        private readonly ILogger<KafkaConsumerHostedService> _logger;
        private readonly IKafkaConsumerProvider _kafkaConsumerProvider;
        private readonly IConfiguration _configuration;

        public KafkaConsumerHostedService(IKafkaConsumerProvider kafkaConsumerProvider, ILogger<KafkaConsumerHostedService> logger, IConfiguration configuration)
        {
            _kafkaConsumerProvider = kafkaConsumerProvider;
            _logger = logger;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using (IKafkaConsumer<Ignore, string> c = this._kafkaConsumerProvider.GetKafkaConsumer<Ignore, string>())
            {
                c.Subscribe(_configuration["kafka-consumer-topic"]);

                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = c.Consume(cancellationToken);
                            _logger.LogInformation($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                        }
                        catch (ConsumeException e)
                        {
                            _logger.LogError($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException e)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    _logger.LogError($"OperationCanceledException occured: {e.Message}");
                    c.Close();
                }
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
