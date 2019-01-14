using System;
using System.Diagnostics;
using Akka.Actor;
using pattern_managed_queue.Messages;

namespace pattern_managed_queue.Actors
{
    public class Producer : UntypedActor {
        private readonly IActorRef Manager;
        private int PendingJobs;
        private double Result;
        private readonly Stopwatch Sw = new Stopwatch();

        public Producer(IActorRef manager) {
            Manager = manager;
        }

        protected override void OnReceive(object message) {
            switch(message) {
                case ComputeAlternatingHarmonicSeries s:
                    Console.WriteLine($"Producer< asked to compute AHS of {s.Value}.");
                    StartComputation(s.Value);
                    break;
                case JobResult jobResult:
                    Console.WriteLine($"Producer< received result {jobResult.Result} from {Context.Sender.Path.Name} ({PendingJobs-1} left)");
                    ProcessJobResult(jobResult);
                    break;
                case JobRejected jobRejected:
                    Console.WriteLine($"Producer< retry job {jobRejected.Id}");
                    RetryJob(jobRejected.Id);
                    break;
                default:
                    Console.WriteLine($"Producer<{Sender.Path.Name} Invalid message {message}");
                    break;
            }
        }

        private void RetryJob(int id) {
            SendJob(id);
        }

        private void StartComputation(int value) {
            Sw.Start();
            for (var i = 1; i <= value; i++) {
                PendingJobs++;
                SendJob(i);
            }
        }

        private void SendJob(int i) {
            Manager.Tell(new Job {
                Id = i,
                Data = i,
                ReplyTo = Self
            });
        }

        private void ProcessJobResult(JobResult jobResult) {
            PendingJobs--;
            Result += jobResult.Result;
            if (PendingJobs == 0) {
                Sw.Stop();
                Console.WriteLine($"Producer: Computation is done with {Result} ({Sw.ElapsedMilliseconds}ms)");
                Context.System.Terminate();
            }
        }
    }
}