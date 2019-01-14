using Akka.Actor;

namespace pattern_managed_queue.Messages
{
    public struct JobRequest
    {
        public IActorRef Requester;
        public int JobSize;
    }
}