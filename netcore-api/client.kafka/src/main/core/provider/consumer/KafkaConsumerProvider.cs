using System;
using System.Collections.Generic;
using System.Text;

namespace client.kafka.consumer.core
{
   

    public class KafkaConsumerProvider : IKafkaConsumerProvider
    {

        private readonly IConsumerBuilderProvider consumerBuilderProvider;
        public KafkaConsumerProvider(IConsumerBuilderProvider consumerBuilderProvider)
        {
            this.consumerBuilderProvider = consumerBuilderProvider;
        }
        public IKafkaConsumer<TKey, TValue> GetKafkaConsumer<TKey, TValue>()
        {
            return new KafkaConsumer<TKey, TValue>(this.consumerBuilderProvider.GetConsumerBuilder<TKey, TValue>().Build());
        }
    }

}
