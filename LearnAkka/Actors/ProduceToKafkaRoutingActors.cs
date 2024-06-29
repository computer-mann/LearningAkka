using Akka.Actor;
using Confluent.Kafka.Admin;
using Confluent.Kafka;
using LearnAkka.Services;
using SharedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnAkka.Actors
{
    public class ProduceToKafkaRoutingActors:ReceiveActor
    {
        private readonly IProducerService _producerService;
        private readonly ILogger<ProduceToKafkaRoutingActors> _logger;

        public ProduceToKafkaRoutingActors(IProducerService producerService, ILogger<ProduceToKafkaRoutingActors> logger)
        {
            _producerService = producerService;
            using var admin = new AdminClientBuilder(new AdminClientConfig
            {
                BootstrapServers = "localhost:8098,localhost:8097,localhost:8099",

            }).Build();
            admin.CreateTopicsAsync(new[]
             {
                        new TopicSpecification
                        {
                            Name = "mamprobi_people",
                            NumPartitions = 3,
                            ReplicationFactor = 3,
                            Configs = new Dictionary<string, string>
                            {
                                {"retention.ms", "86400"}
                            }
                        }
                    });
            ReceiveAsync<MamprobiPeople>(ReceivePerson);
            _logger = logger;
        }
        public async Task ReceivePerson(MamprobiPeople person)
        {
            _logger.LogInformation("the actor router path is {path}",Context.Self.Path.ToString());
            await _producerService.ProduceToKafkaAsync(person);
        }

    }
}
