using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace client.kafka.consumer.core
{
    public class ConsumerConfigProvider : IConsumerConfigProvider
    {
        private readonly IConfiguration configuration;
        public ConsumerConfigProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public IEnumerable<KeyValuePair<string, string>> GetConsumerConfig()
        {
            return new ConsumerConfig { BootstrapServers = configuration["kafka-brokers"], GroupId = "test-consumer-group", AutoOffsetReset = AutoOffsetReset.Earliest };
        }
    }
}
