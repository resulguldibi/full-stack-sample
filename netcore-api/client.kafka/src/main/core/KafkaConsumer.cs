using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Threading;

namespace client.kafka.consumer.core
{
    public class KafkaConsumer<TKey, TValue> : IKafkaConsumer<TKey, TValue>
    {
        private readonly IConsumer<TKey, TValue> consumer;
        private readonly IEnumerable<string> topics;
        public KafkaConsumer(IConsumer<TKey, TValue> consumer,IEnumerable<string> topics)
        {
            this.consumer = consumer;
            this.topics = topics;
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

        public void Subscribe()
        {
            this.consumer.Subscribe(this.topics);
        }
    }
}
