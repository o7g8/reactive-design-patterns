using System;
using Akka.Actor;
using pattern_pull.Actors;
using pattern_pull.Messages;

namespace pattern_pull
{
    class Program
    {
        static void Main(string[] args)
        {
            var actorSystem = ActorSystem.Create("pattern-pull");
            var manager = actorSystem.ActorOf<Manager>("manager");
            manager.Tell(new ComputeAlternatingHarmonicSeries {Value = 1000});
            
            Console.Read();
            actorSystem.Stop(manager);
        }
    }
}
