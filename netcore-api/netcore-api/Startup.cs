using client.kafka.consumer.core;
using client.kafka.producer.core;
using client.socket.core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace netcore_api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IKafkaProducerProvider, KafkaProducerProvider>();
            services.AddSingleton<IProducerBuilderProvider, ProducerBuilderProvider>();
            services.AddSingleton<IProducerConfigProvider, ProducerConfigProvider>();

            services.AddSingleton<IKafkaConsumerProvider, KafkaConsumerProvider>();
            services.AddSingleton<IConsumerBuilderProvider, ConsumerBuilderProvider>();
            services.AddSingleton<IConsumerConfigProvider, ConsumerConfigProvider>();

            //services.AddHostedService<KafkaConsumerHostedService>();

            services.AddControllers();
            services.AddWebSocketManager();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            var serviceProvider = serviceScopeFactory.CreateScope().ServiceProvider;

            app.UseWebSockets();

            app.MapWebSocketManager("/ws", serviceProvider.GetService<SampleSocketMessageHandler>());

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
