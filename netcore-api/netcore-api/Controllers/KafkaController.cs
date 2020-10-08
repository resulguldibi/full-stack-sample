using client.kafka.producer.core;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using netcore_api.Models;
using System;
using System.Threading.Tasks;

namespace netcore_api.Controllers
{
    [ApiController]
    [Route("api/[action]")]
    public class KafkaController : ControllerBase
    {
        private readonly ILogger<KafkaController> _logger;
        private readonly IKafkaProducerProvider _kafkaProducerProvider;
        private readonly IConfiguration _configuration;
        public KafkaController(ILogger<KafkaController> logger, IKafkaProducerProvider kafkaProducerProvider, IConfiguration configuration)
        {
            _logger = logger;
            _kafkaProducerProvider = kafkaProducerProvider;
            _configuration = configuration;
        }


        [HttpPost]
        [ActionName("messages/async")]
        public async Task<PersistenceStatus> CreateMessageAsync([FromBody] KafkaMessageModel request)
        {
            DeliveryResult<Null, string> dr = null;
            using (IKafkaProducer<Null, string> p = this._kafkaProducerProvider.GetKafkaProducer<Null, string>())
            {
                try
                {
                    dr = await p.ProduceAsync(_configuration["kafka-producer-topic"], new Message<Null, string> { Value = request.Message });
                    _logger.LogInformation($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                }
                catch (ProduceException<Null, string> e)
                {
                    _logger.LogError($"Delivery failed: {e.Error.Reason}");
                }
            }

            return dr.Status;
        }

        [HttpPost]
        [ActionName("messages/sync")]
        public PersistenceStatus CreateMessageSync([FromBody] KafkaMessageModel request)
        {
            using (IKafkaProducer<Null, string> p = this._kafkaProducerProvider.GetKafkaProducer<Null, string>())
            {
                try
                {
                    Action<DeliveryReport<Null, string>> handler = r =>
            _logger.LogInformation(!r.Error.IsError
                ? $"Delivered message to {r.TopicPartitionOffset}"
                : $"Delivery Error: {r.Error.Reason}");
                    p.Produce(_configuration["kafka-producer-topic"], new Message<Null, string> { Value = request.Message }, handler);
                    p.Flush(TimeSpan.FromMilliseconds(100));
                }
                catch (ProduceException<Null, string> e)
                {
                    _logger.LogError($"Delivery failed: {e.Error.Reason}");
                }
            }

            return PersistenceStatus.PossiblyPersisted;
        }
    }
}
