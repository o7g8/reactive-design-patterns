namespace pattern_managed_queue.Messages
{
    public struct JobRejected
    {
        public JobRejected(int id) => Id = id;

        public int Id { get; }
    }
}