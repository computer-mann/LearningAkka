using Confluent.Kafka;
using KafkaFlow;
using SharedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LearnAkka.Services
{
    public class ProducerService : IProducerService
    {
        private readonly IMessageProducer<ProducerService> _messageProducer;
        private readonly ILogger<ProducerService> _logger;

        public ProducerService(IMessageProducer<ProducerService> messageProducer, ILogger<ProducerService> logger)
        {
            _messageProducer = messageProducer;
            _logger = logger;
        }

        public async Task ProduceToKafkaAsync(MamprobiPeople people)
        {
            var result = await _messageProducer.ProduceAsync(people.Id, people);
           if (result.Status != PersistenceStatus.Persisted)
            {
                _logger.LogWarning("Failed to produce message to Kafka {@people}",people);
            }else
            {
                _logger.LogInformation("Produced message to Kafka {@People}",JsonSerializer.Serialize(people));
            }
            
        }
    }
    public interface IProducerService
    {
        Task ProduceToKafkaAsync(MamprobiPeople people);
    }
}
