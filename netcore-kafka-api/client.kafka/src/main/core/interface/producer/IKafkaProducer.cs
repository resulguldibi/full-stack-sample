using Confluent.Kafka;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace client.kafka.producer.core
{
    public interface IKafkaProducer<TKey, TValue> : IDisposable
    {
        Task<DeliveryResult<TKey, TValue>> ProduceAsync(string topic, Message<TKey, TValue> message, CancellationToken cancellationToken = default);
        void Produce(string topic, Message<TKey, TValue> message, Action<DeliveryReport<TKey, TValue>> deliveryHandler = null);

        int Flush(TimeSpan timeout);
    }
}
