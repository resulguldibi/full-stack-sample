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
    public class SelectionController : ControllerBase
    {
        private readonly ILogger<SelectionController> _logger;
        private readonly IKafkaProducerProvider _kafkaProducerProvider;
        private readonly IConfiguration _configuration;
        public SelectionController(ILogger<SelectionController> logger, IKafkaProducerProvider kafkaProducerProvider, IConfiguration configuration)
        {
            _logger = logger;
            _kafkaProducerProvider = kafkaProducerProvider;
            _configuration = configuration;
        }


        [HttpPost]
        [ActionName("selections/async")]
        public async Task<PersistenceStatus> CreateMessageAsync([FromBody] SaveSelectionRequestModel request)
        {
            DeliveryResult<Null, string> dr = null;
            using (IKafkaProducer<Null, string> p = this._kafkaProducerProvider.GetKafkaProducer<Null, string>("producer-1"))
            {
                try
                {
                    dr = await p.ProduceAsync(new Message<Null, string> { Value = request.ToJSON() });
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
        [ActionName("selections/sync")]
        public PersistenceStatus CreateMessageSync([FromBody] SaveSelectionRequestModel request)
        {
            using (IKafkaProducer<Null, string> p = this._kafkaProducerProvider.GetKafkaProducer<Null, string>("producer-1"))
            {
                try
                {
                    Action<DeliveryReport<Null, string>> handler = r =>
            _logger.LogInformation(!r.Error.IsError
                ? $"Delivered message to {r.TopicPartitionOffset}"
                : $"Delivery Error: {r.Error.Reason}");
                    p.Produce(new Message<Null, string> { Value = request.ToJSON() }, handler);
                    p.Flush(TimeSpan.FromMilliseconds(100));
                }
                catch (ProduceException<Null, string> e)
                {
                    _logger.LogError($"Delivery failed: {e.Error.Reason}");
                }
            }

            return PersistenceStatus.PossiblyPersisted;
        }


        [HttpPost]
        [ActionName("selections/synchronization")]
        public async Task<PersistenceStatus> SynchronizeSelectionSummary([FromBody] SynchronizeSelectionSummaryRequestModel request)
        {            
            DeliveryResult<Null, string> dr = null;
            using (IKafkaProducer<Null, string> p = this._kafkaProducerProvider.GetKafkaProducer<Null, string>("producer-synchronization"))
            {
                try
                {
                    dr = await p.ProduceAsync(new Message<Null, string> { Value = request.ToJSON() });
                    _logger.LogInformation($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                }
                catch (ProduceException<Null, string> e)
                {
                    _logger.LogError($"Delivery failed: {e.Error.Reason}");
                }
            }

            return dr.Status;
        }
    }
}
