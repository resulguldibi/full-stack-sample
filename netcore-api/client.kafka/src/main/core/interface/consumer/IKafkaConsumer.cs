using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Threading;

namespace client.kafka.consumer.core
{
    public interface IKafkaConsumer<TKey, TValue> : IDisposable
    {
        ConsumeResult<TKey, TValue> Consume(CancellationToken cancellationToken = default);
        
        ConsumeResult<TKey, TValue> Consume(TimeSpan timeout);
       
        ConsumeResult<TKey, TValue> Consume(int millisecondsTimeout);

        void Subscribe(string topic);

        void Subscribe(IEnumerable<string> topics);
        void Close();
    }
}
