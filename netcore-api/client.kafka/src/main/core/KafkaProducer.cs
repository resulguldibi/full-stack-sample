using Confluent.Kafka;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace client.kafka.producer.core
{
    public class KafkaProducer<TKey, TValue> : IKafkaProducer<TKey, TValue>
    {
        private readonly IProducer<TKey, TValue> producer;
        public KafkaProducer(IProducer<TKey, TValue> producer)
        {
            this.producer = producer;
        }

        public void Dispose()
        {
            this.producer?.Dispose();
        }

        public int Flush(TimeSpan timeout)
        {
            return this.producer.Flush(timeout);
        }

        public void Produce(string topic, Message<TKey, TValue> message, Action<DeliveryReport<TKey, TValue>> deliveryHandler = null)
        {
            this.producer.Produce(topic, message, deliveryHandler);
        }

        public async Task<DeliveryResult<TKey, TValue>> ProduceAsync(string topic, Message<TKey, TValue> message, CancellationToken cancellationToken = default)
        {
            return await this.producer.ProduceAsync(topic, message, cancellationToken);
        }
    }
}
