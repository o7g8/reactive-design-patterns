using System;
using System.Threading;
using Akka.Actor;
using pattern_pull.Messages;

namespace pattern_pull.Actors
{
    public class Worker : UntypedActor {
        private const int REQUEST_SIZE = 10;
        private const int JOB_LOW_LIMIT = 5;
        private int Requested;

        protected override void OnReceive(object message) {
            switch(message) {
                case Job job:
                    Console.WriteLine($"{Self.Path.Name}> computing {job.Data}.");
                    Request(job.ReplyTo);
                    var result = Compute(job.Data);
                    job.ReplyTo.Tell(new JobResult {Id = job.Id, Result = result}, Self);
                    break;
                default:
                    Console.WriteLine($"{Self.Path.Name}> unknown message {message}.");
                    break;
            }
        }

        private void Request(IActorRef manager) {
            Requested--;
            if(Requested > JOB_LOW_LIMIT) {
                return;
            }
            Console.WriteLine($"{Self.Path.Name}> low on work {Requested}.");
            Requested += REQUEST_SIZE;
            manager.Tell(new JobRequest {Requester = Self, JobSize = REQUEST_SIZE});
        }

        private double Compute(int data) {
            double sign = data % 2 == 1? 1 : -1;
            return sign / data; 
        }
    }
}