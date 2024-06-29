using Akka.Actor;
using Akka.Hosting;
using Akka.Routing;
using Bogus;
using LearnAkka.Actors;
using SharedModels;
using UuidExtensions;
using static LearnAkka.Actors.ProduceToKafkaRoutingActors;

namespace LearnAkka
{
    public class ProducerBackgroundWorker : BackgroundService
    {
        private readonly ILogger<ProducerBackgroundWorker> _logger;
        private readonly IRequiredActor<ProduceToKafkaRoutingActors> _firstActor;

        public ProducerBackgroundWorker(ILogger<ProducerBackgroundWorker> logger, IRequiredActor<ProduceToKafkaRoutingActors> requiredActor)
        {
            _logger = logger;
            _firstActor = requiredActor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("in the worker");
            Randomizer.Seed = new Random(8675309);
            
            try
            {
                var actor =await _firstActor.GetAsync(stoppingToken);
                while (!stoppingToken.IsCancellationRequested)
                {
                    var testUsers = new Bogus.Person();
                   
                        var person = new MamprobiPeople
                        {
                            Id=Uuid7.Id25(),
                            Address=testUsers.Address.Street,
                            Arrival=DateOnly.FromDateTime(DateTime.MinValue.AddYears(Random.Shared.Next(1000))),
                            FirstName=testUsers.FirstName,
                            LastName=testUsers.LastName
                        };
                    var msg=new ConsistentHashableEnvelope(person,person.Id);
                        actor.Tell(msg,ActorRefs.NoSender);
                    
                    await Task.Delay(200, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "errorrrrrrrr");
            }
        }
    }
}
