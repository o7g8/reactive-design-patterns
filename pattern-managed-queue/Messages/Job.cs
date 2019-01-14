using Akka.Actor;

namespace pattern_managed_queue.Messages
{
    public struct Job
    {
        public int Id;
        public int Data;
        public IActorRef ReplyTo;
    }
}