using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnAkka.Actors
{
    public class ProduceToKafkaRoutingActors:ReceiveActor
    {
        public ProduceToKafkaRoutingActors()
        {
            Receive<ActingPerson>(ReceivePerson);
        }
        public void ReceivePerson(ActingPerson person)
        {
            Console.WriteLine("Received Person message: {0}", person.Name);
        }

        public struct ActingPerson
        {
            public string Name;
        }
    }
}
