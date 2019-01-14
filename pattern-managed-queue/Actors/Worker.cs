using System;
using Akka.Actor;
using pattern_managed_queue.Messages;

namespace pattern_managed_queue.Actors
{
    public class Worker : UntypedActor {
        private const int REQUEST_SIZE = 10;
        private const int JOB_LOW_LIMIT = 5;
        private readonly IActorRef Manager;
        private int Requested;

        public Worker(IActorRef manager) {
            Manager = manager;
        }

        protected override void OnReceive(object message) {
            switch(message) {
                case Job job:
                    Console.WriteLine($"{Self.Path.Name}> computing {job.Data}.");
                    RequestHavingReceived(1);
                    var result = Compute(job.Data);
                    job.ReplyTo.Tell(new JobResult {Id = job.Id, Result = result}, Self);
                    break;
                case DummyJob dummyJob:
                    Console.WriteLine($"{Self.Path.Name}> dummy job {dummyJob.Count}.");
                    RequestHavingReceived(dummyJob.Count);
                    break;
                default:
                    Console.WriteLine($"{Self.Path.Name}> unknown message {message}.");
                    break;
            }
        }

        protected override void PreStart() {
            RequestHavingReceived(0);
        }

        private void RequestHavingReceived(int receivedJobs) {
            Requested -= receivedJobs;
            if(Requested > JOB_LOW_LIMIT) {
                return;
            }
            Console.WriteLine($"{Self.Path.Name}> low on work {Requested}.");
            Requested += REQUEST_SIZE;
            Manager.Tell(new JobRequest {Requester = Self, JobSize = REQUEST_SIZE});
        }

        private double Compute(int data) {
            double sign = ((data % 2) == 1) ? 1 : -1;
            return sign / data; 
        }
    }
}