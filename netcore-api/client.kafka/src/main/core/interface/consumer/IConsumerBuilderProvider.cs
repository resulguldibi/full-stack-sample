using Confluent.Kafka;

namespace client.kafka.consumer.core
{
    public interface IConsumerBuilderProvider
    {
        ConsumerBuilder<TKey, TValue> GetConsumerBuilder<TKey, TValue>();
    }
}
