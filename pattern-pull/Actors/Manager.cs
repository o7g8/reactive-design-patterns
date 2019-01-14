using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Akka.Actor;
using pattern_pull.Messages;

namespace pattern_pull.Actors
{
    /* The Manager doesn't handle situation with undelivered messages to/from workers,
       therefore there are risks of:
       - "starwing" workers out of job;
       - not received computation results and hence overall computation is never done. */
    public class Manager : UntypedActor {
        private readonly List<IActorRef> Workers = new List<IActorRef>();
        private Queue<Job> JobStream;
        private int PendingJobs;
        private double Result;
        private Stopwatch Sw = new Stopwatch();

        public Manager() {
            CreateActors();
        }

        protected override void OnReceive(object message) {
            switch(message) {
                case ComputeAlternatingHarmonicSeries s:
                    Console.WriteLine($"Manager: asking to compute AHS of {s.Value}.");
                    JobStream = new Queue<Job>(Enumerable
                                .Range(1, s.Value)
                                .Select(i => new Job {Id = i, Data = i, ReplyTo = Self}));
                    Sw.Start();
                    StartComputation();
                    break;
                case JobResult jobResult:
                    Console.WriteLine($"Manager< received result {jobResult.Result} from {Context.Sender.Path.Name}");
                    ProcessJobResult(jobResult);
                    break;
                case JobRequest jobRequest:
                    Console.WriteLine($"Manager< received job request of {jobRequest.JobSize} from {jobRequest.Requester.Path.Name}");
                    ScheduleJob(jobRequest);
                    break;
                default:
                    Console.WriteLine($"Manager< Invalid message {message}");
                    break;
            }
        }

        private void ScheduleJob(JobRequest jobRequest) {
            for(var i = 0; i < jobRequest.JobSize && JobStream.Count > 0; i++) {
                SendJob(jobRequest.Requester);
            }
        }

        private void ProcessJobResult(JobResult jobResult) {
            Result += jobResult.Result;
            PendingJobs--;
            if (JobStream.Count == 0 && PendingJobs == 0) {
                Sw.Stop();
                Console.WriteLine($"Manager: Computation is done with {Result} ({Sw.ElapsedMilliseconds}ms)");
                Context.System.Terminate();
            }
        }

        private void StartComputation() {
            foreach(var worker in Workers) {
                SendJob(worker);
            }
        }

        private void SendJob(IActorRef worker) {
            if (JobStream.Count > 0) {
                PendingJobs++;
                Console.WriteLine($"Manager> sending job to {worker.Path.Name}");
                worker.Tell(JobStream.Dequeue(), Self);
            }
        }

        private void CreateActors() {
            if(Workers.Any()) {
                return;
            }
            for(var i = 0; i < 3; i++) {
                Workers.Add(Context.ActorOf<Worker>($"worker{i}"));
                Console.WriteLine($"Created worker{i}");
            }
        }
    }
}