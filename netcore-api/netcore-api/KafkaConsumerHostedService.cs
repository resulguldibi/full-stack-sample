using client.kafka.consumer.core;
using client.socket.core;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using netcore_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace netcore_api
{
    public class KafkaConsumerHostedService : IHostedService
    {
        private readonly ILogger<KafkaConsumerHostedService> _logger;
        private readonly IKafkaConsumerProvider _kafkaConsumerProvider;
        private readonly IConfiguration _configuration;
        private readonly SampleSocketMessageHandler _sampleSocketMessageHandler;

        public KafkaConsumerHostedService(IKafkaConsumerProvider kafkaConsumerProvider, SampleSocketMessageHandler sampleSocketMessageHandler, ILogger<KafkaConsumerHostedService> logger, IConfiguration configuration)
        {
            _kafkaConsumerProvider = kafkaConsumerProvider;
            _logger = logger;
            _configuration = configuration;
            _sampleSocketMessageHandler = sampleSocketMessageHandler;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {

                using (IKafkaConsumer<Ignore, string> c = this._kafkaConsumerProvider.GetKafkaConsumer<Ignore, string>("consumer-selection-count"))
                {
                    c.Subscribe();

                    try
                    {
                        while (true)
                        {
                            try
                            {
                                var cr = c.Consume(cancellationToken);
                                _logger.LogInformation($"1. Consumed message '{cr.Message.Value}' at: '{cr.TopicPartitionOffset}'.");
                                SelectionSummaryKafkaMessage kafkaSocketMessage = null;
                                try
                                {
                                    kafkaSocketMessage = cr.Message?.Value?.FromJSON<SelectionSummaryKafkaMessage>();
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError($"Exception occured in KafkaSocketMessage deserialization: {ex.Message}");
                                }

                                if (kafkaSocketMessage != null)
                                {
                                    SocketMessage calculationResultMessage = new SocketMessage()
                                    {
                                        Code = "selection_count_calculated",
                                        Data = kafkaSocketMessage
                                    };

                                    Task.Run(async () => { await _sampleSocketMessageHandler.SendMessageAsync(kafkaSocketMessage.UserId, calculationResultMessage.ToJSON()); });
                                }
                            }
                            catch (ConsumeException e)
                            {
                                _logger.LogError($"Error occured: {e.Error.Reason}");
                            }
                        }
                    }
                    catch (OperationCanceledException e)
                    {
                        _logger.LogError($"OperationCanceledException occured: {e.Message}");
                        c.Close();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Exception occured: {ex.Message}");
                    }
                }

            }, cancellationToken);

            Task.Run(() =>
            {

                using (IKafkaConsumer<Ignore, string> c = this._kafkaConsumerProvider.GetKafkaConsumer<Ignore, string>("consumer-selection-synchronization"))
                {
                    c.Subscribe();

                    try
                    {
                        while (true)
                        {
                            try
                            {
                                var cr = c.Consume(cancellationToken);
                                _logger.LogInformation($"2. Consumed message '{cr.Message.Value}' at: '{cr.TopicPartitionOffset}'.");
                                SelectionSummaryKafkaMessage[] kafkaSocketMessage = null;
                                try
                                {
                                    kafkaSocketMessage = cr.Message?.Value?.FromJSON<SelectionSummaryKafkaMessage[]>();
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError($"Exception occured in KafkaSocketMessage deserialization: {ex.Message}");
                                }

                                if (kafkaSocketMessage != null)
                                {
                                    SocketMessage calculationResultMessage = new SocketMessage()
                                    {
                                        Code = "selection_synchronization_calculated",
                                        Data = kafkaSocketMessage
                                    };

                                    Task.Run(async () => { await _sampleSocketMessageHandler.SendMessageAsync(kafkaSocketMessage.First().UserId, calculationResultMessage.ToJSON()); });
                                }
                            }
                            catch (ConsumeException e)
                            {
                                _logger.LogError($"Error occured: {e.Error.Reason}");
                            }
                        }
                    }
                    catch (OperationCanceledException e)
                    {
                        _logger.LogError($"OperationCanceledException occured: {e.Message}");
                        c.Close();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Exception occured: {ex.Message}");
                    }
                }

            }, cancellationToken);

            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public static class JsonSerialization
    {
        public static string ToJSON<T>(this T data)
        {
            return JsonSerializer.Serialize<T>(data);
        }

        public static T FromJSON<T>(this string data)
        {
            return JsonSerializer.Deserialize<T>(data);
        }
    }

    public static class HostedServiceExtensions
    {        
        public static IServiceCollection AddHostedServices(this IServiceCollection services, List<Assembly> workersAssemblies)
        {
            MethodInfo methodInfo =
                typeof(ServiceCollectionHostedServiceExtensions)
                .GetMethods()
                .FirstOrDefault(p => p.Name == nameof(ServiceCollectionHostedServiceExtensions.AddHostedService));

            if (methodInfo == null)
                throw new Exception($"Impossible to find the extension method '{nameof(ServiceCollectionHostedServiceExtensions.AddHostedService)}' of '{nameof(IServiceCollection)}'.");

            IEnumerable<Type> hostedServices_FromAssemblies = workersAssemblies.SelectMany(a => a.DefinedTypes).Where(x => x.GetInterfaces().Contains(typeof(IHostedService))).Select(p => p.AsType());

            foreach (Type hostedService in hostedServices_FromAssemblies)
            {
                if (typeof(IHostedService).IsAssignableFrom(hostedService))
                {
                    var genericMethod_AddHostedService = methodInfo.MakeGenericMethod(hostedService);
                    _ = genericMethod_AddHostedService.Invoke(obj: null, parameters: new object[] { services }); // this is like calling services.AddHostedService<T>(), but with dynamic T (= backgroundService).
                }
            }

            return services;
        }
    }

}
