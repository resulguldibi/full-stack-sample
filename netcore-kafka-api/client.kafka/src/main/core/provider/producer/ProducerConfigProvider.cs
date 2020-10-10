using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace client.kafka.producer.core
{
    public class ProducerConfigProvider : IProducerConfigProvider
    {
        private readonly IConfiguration configuration;
        public ProducerConfigProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public IEnumerable<KeyValuePair<string, string>> GetProducerConfig()
        {
            return new ProducerConfig { BootstrapServers = configuration["kafka-brokers"] };
        }
    }
}
