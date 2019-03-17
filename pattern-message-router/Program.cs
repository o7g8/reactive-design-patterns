using System;
using Akka.Actor;

namespace pattern_message_router
{
    class Program {
        static void Main(string[] args) {
            using(var actorSystem = ActorSystem.Create("pattern-message-router")) {
                var processor1 = actorSystem.ActorOf<Processor>("processor1");
                var processor2 = actorSystem.ActorOf<Processor>("processor2");
                var router = actorSystem.ActorOf(
                    Props.Create(() => new AlternatingRouter(processor1, processor2)));

                for(var i = 1; i <= 10; i++) {
                    router.Tell($"Message #{i}");
                }
                
                Console.Read();
            };
        }
    }
}
