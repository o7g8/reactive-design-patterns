using System;
using Akka.Actor;
using pattern_managed_queue.Actors;
using pattern_managed_queue.Messages;

namespace pattern_managed_queue
{
    class Program
    {
        static void Main(string[] args)
        {
            var actorSystem = ActorSystem.Create("pattern_managed_queue");
            var manager = actorSystem.ActorOf<Manager>("manager");
            var producer = actorSystem.ActorOf(Props.Create<Producer>(manager), "producer");
            producer.Tell(new ComputeAlternatingHarmonicSeries { Value = 10000 });

            Console.ReadKey();
            actorSystem.Stop(producer);
            actorSystem.Stop(manager);
        }
    }
}
