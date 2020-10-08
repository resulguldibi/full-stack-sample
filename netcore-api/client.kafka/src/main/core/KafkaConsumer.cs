using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Threading;

namespace client.kafka.consumer.core
{
    public class KafkaConsumer<TKey, TValue> : IKafkaConsumer<TKey, TValue>
    {
        private readonly IConsumer<TKey, TValue> consumer;
        public KafkaConsumer(IConsumer<TKey, TValue> consumer)
        {
            this.consumer = consumer;
        }

        public void Close()
        {
            this.consumer.Close();
        }

        public ConsumeResult<TKey, TValue> Consume(CancellationToken cancellationToken = default)
        {
            return this.consumer.Consume(cancellationToken);
        }

        public ConsumeResult<TKey, TValue> Consume(TimeSpan timeout)
        {
            return this.consumer.Consume(timeout);
        }

        public ConsumeResult<TKey, TValue> Consume(int millisecondsTimeout)
        {
            return this.consumer.Consume(millisecondsTimeout);
        }

        public void Dispose()
        {
            this.consumer?.Dispose();
        }

        public void Subscribe(string topic)
        {
            this.consumer.Subscribe(topic);
        }

        public void Subscribe(IEnumerable<string> topics)
        {
            this.consumer.Subscribe(topics);
        }
    }
}
