using System;
using Akka.Actor;

namespace pattern_message_router
{
    public class Processor : UntypedActor {
        protected override void OnReceive(object message) {
            Console.WriteLine($"Processor {Self.Path.Name} received {message}");
        }
    }

    public class AlternatingRouter : UntypedActor {
        private readonly IActorRef Processor1;
        private readonly IActorRef Processor2;
        private bool UseProcessor1;

        public AlternatingRouter(IActorRef processor1, IActorRef processor2) {
            Processor1 = processor1;
            Processor2 = processor2;
        }

        protected override void OnReceive(object message) {
            var processor = AlternateProcessor();
            Console.WriteLine($"AlternatingRouter: routing {message} to {processor.Path.Name}");
            processor.Tell(message);
        }

        private IActorRef AlternateProcessor() {
            UseProcessor1 = !UseProcessor1;
            return UseProcessor1 ? Processor1 : Processor2;
        }
    }
}
