namespace client.kafka.producer.core
{
    public class KafkaProducerProvider : IKafkaProducerProvider
    {

        private readonly IProducerBuilderProvider producerBuilderProvider;
        public KafkaProducerProvider(IProducerBuilderProvider producerBuilderProvider)
        {
            this.producerBuilderProvider = producerBuilderProvider;
        }
        public IKafkaProducer<TKey, TValue> GetKafkaProducer<TKey, TValue>()
        {
            return new KafkaProducer<TKey, TValue>(this.producerBuilderProvider.GetProducerBuilder<TKey, TValue>().Build());
        }
    }
}
