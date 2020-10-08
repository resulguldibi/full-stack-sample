using Confluent.Kafka;

namespace client.kafka.consumer.core
{
    public class ConsumerBuilderProvider : IConsumerBuilderProvider
    {
        private readonly IConsumerConfigProvider consumerConfigProvider;
        public ConsumerBuilderProvider(IConsumerConfigProvider consumerConfigProvider)
        {
            this.consumerConfigProvider = consumerConfigProvider;
        }

        public ConsumerBuilder<TKey, TValue> GetConsumerBuilder<TKey, TValue>()
        {
            return new ConsumerBuilder<TKey, TValue>(this.consumerConfigProvider.GetConsumerConfig());
        }
    }
}
