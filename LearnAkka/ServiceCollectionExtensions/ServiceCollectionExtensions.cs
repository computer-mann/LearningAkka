using Akka.Hosting;
using Akka.Actor;
using Akka.Routing;
using LearnAkka.Actors;

namespace LearnAkka.ServiceCollectionExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAkkas(this IServiceCollection services)
        {
            services.AddAkka("AkkaOne", builder =>
            {
                builder.WithActors((system, registry, resolver) =>
                {
                    var defaultStrategy = new OneForOneStrategy(
                        3, 3_000, ex =>
                        {
                            if (ex is not ActorInitializationException)
                                return Directive.Resume;

                            system?.Terminate().Wait(1000);

                            return Directive.Stop;
                        }, false);
                    var first = resolver.Props<FirstActor>().WithSupervisorStrategy(defaultStrategy);
                    var one = system.ActorOf(first, nameof(first));
                    registry.Register<FirstActor>(one);

                    var rout = resolver.Props<ProduceToKafkaRoutingActors>().WithRouter(new ConsistentHashingPool(5));
                    var two = system.ActorOf(rout, nameof(rout));
                    registry.Register<ProduceToKafkaRoutingActors>(two);
                });
            });
        }
    }
}
