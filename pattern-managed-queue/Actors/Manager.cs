using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using pattern_managed_queue.Messages;

namespace pattern_managed_queue.Actors
{
    /* The Manager doesn't handle situation with undelivered messages to/from workers,
       therefore there are risks of:
       - "starving" workers out of job;
       - not received computation results and hence overall computation is never done. */
    public class Manager : UntypedActor {
        private const int MAX_JOBS = 1000;
        private readonly List<IActorRef> Workers = new List<IActorRef>();
        private readonly Queue<Job> JobQueue = new Queue<Job>();
        private readonly Queue<JobRequest> RequestQueue = new Queue<JobRequest>();

        public Manager() {
            CreateActors();
        }

        protected override void OnReceive(object message) {
            switch(message) {
                case Job job:
                    Console.WriteLine($"Manager: job received.");
                    if(RequestQueue.Count == 0) {
                        if(JobQueue.Count < MAX_JOBS) {
                            JobQueue.Enqueue(job);
                        } else {
                            // reject new job if our buffer of pending jobs
                            // is full and no consumers are requesting new jobs
                            job.ReplyTo.Tell(new JobRejected(id: job.Id));
                        }
                    } else {
                        var request = RequestQueue.Dequeue();
                        request.Requester.Tell(job);
                        if(request.JobSize > 1) {
                            request.Requester.Tell(new DummyJob{Count= request.JobSize - 1, ReplyTo = Self });
                        }
                    }
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
            if(JobQueue.Count == 0) {
                RequestQueue.Enqueue(jobRequest);
            } else {
                int sent;
                for(sent = 0; sent < jobRequest.JobSize && JobQueue.Count > 0; sent++) {
                    SendJob(jobRequest.Requester);
                }
                if(sent < jobRequest.JobSize) {
                    jobRequest.Requester.Tell(new DummyJob {Count= jobRequest.JobSize - sent, ReplyTo = Self});
                }
            }
        }

        private void StartComputation() {
            foreach(var worker in Workers) {
                SendJob(worker);
            }
        }

        private void SendJob(IActorRef worker) {
            if (JobQueue.Count > 0) {
                Console.WriteLine($"Manager> sending job to {worker.Path.Name}");
                worker.Tell(JobQueue.Dequeue(), Self);
            }
        }

        private void CreateActors() {
            if(Workers.Any()) {
                return;
            }
            for(var i = 0; i < 3; i++) {
                Workers.Add(Context.ActorOf(Props.Create<Worker>(Self), $"worker{i}"));
                Console.WriteLine($"Created worker{i}");
            }
        }
    }
}