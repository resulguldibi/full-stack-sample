using Confluent.Kafka;

namespace client.kafka.producer.core
{
    public class ProducerBuilderProvider : IProducerBuilderProvider
    {
        private readonly IProducerConfigProvider producerConfigProvider;
        public ProducerBuilderProvider(IProducerConfigProvider producerConfigProvider)
        {
            this.producerConfigProvider = producerConfigProvider;
        }

        public ProducerBuilder<TKey, TValue> GetProducerBuilder<TKey, TValue>()
        {
            return new ProducerBuilder<TKey, TValue>(this.producerConfigProvider.GetProducerConfig());
        }
    }
}
