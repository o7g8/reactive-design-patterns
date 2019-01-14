using Akka.Actor;

namespace pattern_managed_queue.Messages {
    internal class DummyJob {
        public int Count;
        public IActorRef ReplyTo;
    }
}