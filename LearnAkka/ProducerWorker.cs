using Akka.Actor;
using Akka.Hosting;
using Bogus;
using LearnAkka.Actors;
using static LearnAkka.Actors.ProduceToKafkaRoutingActors;

namespace LearnAkka
{
    public class ProducerWorker : BackgroundService
    {
        private readonly ILogger<ProducerWorker> _logger;
        private readonly IRequiredActor<ProduceToKafkaRoutingActors> _firstActor;

        public ProducerWorker(ILogger<ProducerWorker> logger, IRequiredActor<ProduceToKafkaRoutingActors> requiredActor)
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
                   
                        var person = new ActingPerson
                        {
                            Name = testUsers.FullName
                        };
                        actor.Tell(person,ActorRefs.NoSender);
                    
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "errorrrrrrrr");
            }
        }
    }
}
