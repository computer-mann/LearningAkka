
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using KafkaFlow;
using LearnAkka.Actors;
using LearnAkka.ServiceCollectionExtensions;
using LearnAkka.Services;
using SharedModels;
using KafkaFlow.Serializer;

namespace LearnAkka
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            var services = builder.Services;
            services.AddHostedService<ProducerWorker>();
            services.AddAkkas();
            services.AddSingleton<IProducerService, ProducerService>();
            services.AddKafka(kafka =>
            {
                kafka.AddCluster(brokers => brokers.WithBrokers(["localhost:8098", "localhost:8097", "localhost:8099"])
                .AddProducer<ProducerService>(options =>
                {
                    options.WithProducerConfig(new ProducerConfig
                    {
                        Acks = Confluent.Kafka.Acks.Leader,
                        AllowAutoCreateTopics = false


                    });
                    options.AddMiddlewares(middleware =>
                    {
                        middleware.AddSingleTypeSerializer<JsonCoreSerializer>(typeof(MamprobiPeople));
                    });
                    options.DefaultTopic("mamprobi_people");
                }).OnStarted(handler =>
                {
                    var logger = services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("Creating topics");
                    
                }));
            });

            var host = builder.Build();
            host.Run();
        }
    }
}