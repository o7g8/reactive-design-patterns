using Akka.Actor;

namespace pattern_pull.Messages
{
    public struct Job
    {
        public int Id;
        public int Data;
        public IActorRef ReplyTo;
    }
}