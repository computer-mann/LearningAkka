using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnAkka.Actors
{
    public class FirstActor : ReceiveActor
    {
        private readonly ILoggingAdapter log = Context.GetLogger();

        public FirstActor()
        {
            Receive<int>(message =>
            {
                log.Info("Received String message: {0}", message);
            });

        }
    }
}
