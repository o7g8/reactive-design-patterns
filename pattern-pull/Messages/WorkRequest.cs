using Akka.Actor;

namespace pattern_pull.Messages
{
    public struct JobRequest
    {
        public IActorRef Requester;
        public int JobSize;
    }
}