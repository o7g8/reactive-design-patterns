using System;
using System.Collections.Generic;
using Akka.Actor;

namespace pattern_pipes_and_filters
{
    # region Messages

    public class ProcessIncomingOrder {
        public string OrderText {get;}
        public ProcessIncomingOrder(string orderText) => OrderText = orderText;
    }

    #endregion Messages

    #region Actors

    public class OrderManagementSystem : ReceiveActor {
        public OrderManagementSystem() {
            Receive<ProcessIncomingOrder>(msg => ProcessOrder(msg));
        }

        private void ProcessOrder(ProcessIncomingOrder order) {
            Console.WriteLine($"OrderManagementSystem: processing unique order: {order.OrderText}");
        }
    }

    public class Deduplicator : ReceiveActor {
        private readonly IActorRef Next;
        private readonly HashSet<string> ProcessedOrders = new HashSet<string>();

        public Deduplicator(IActorRef next) {
            Next = next;
            Receive<ProcessIncomingOrder>(msg => ProcessOrder(msg));
        }

        public static Props Props(IActorRef next) {
            return Akka.Actor.Props.Create(() => new Deduplicator(next));
        }

        private void ProcessOrder(ProcessIncomingOrder order) {
            Console.WriteLine($"Deduplicator: processing order: {order.OrderText}");
            var orderId = GetOrderId(order.OrderText);
            if(!ProcessedOrders.Contains(orderId)) {
                ProcessedOrders.Add(orderId);
                Next.Tell(order);
            } else {
                Console.WriteLine($"Deduplicator: found duplicate order: {orderId}");
            }
        }

        private string GetOrderId(string orderText) {
            var orderIdIndex = orderText.IndexOf("id='") + 4;
            var orderIdLastIndex = orderText.IndexOf("'", orderIdIndex);
            return orderText.Substring(orderIdIndex, orderIdLastIndex - orderIdIndex);
        }
    }

    public class OrderAcceptanceEndpoint : ReceiveActor {
        private readonly IActorRef Next;

        public OrderAcceptanceEndpoint(IActorRef next) {
            Next = next;
            Receive<string>(msg => ProcessOrder(msg));
        }

        public static Props Props(IActorRef next) =>
            Akka.Actor.Props.Create(() => new OrderAcceptanceEndpoint(next));

        private void ProcessOrder(string order) {
            Console.WriteLine($"OrderAcceptanceEndpoint: processing {order}");
            Next.Tell(new ProcessIncomingOrder(order));
        }
    }

    internal class Decrypter : ReceiveActor {
        private readonly IActorRef Next;

        public Decrypter(IActorRef next) {
            Next = next;
            Receive<ProcessIncomingOrder>(msg => ProcessOrder(msg));
        }

        public static Props Props(IActorRef next) =>
            Akka.Actor.Props.Create(() => new Decrypter(next));

        private void ProcessOrder(ProcessIncomingOrder order) {
            Console.WriteLine($"Decrypter: processing {order.OrderText}");
            var decrypted = order.OrderText.Replace("(encryption)", "");
            Next.Tell(new ProcessIncomingOrder(decrypted));
        }
    }

    internal class Authenticator : ReceiveActor {
        private IActorRef Next;

        public Authenticator(IActorRef next) {
            Next = next;
            Receive<ProcessIncomingOrder>(msg => ProcessOrder(msg));
        }
        internal static Props Props(IActorRef next) =>
            Akka.Actor.Props.Create(() => new Authenticator(next));

        private void ProcessOrder(ProcessIncomingOrder order) {
            Console.WriteLine($"Authenticator: processing {order.OrderText}");
            var decrypted = order.OrderText.Replace("(certificate)", "");
            Next.Tell(new ProcessIncomingOrder(decrypted));
        }
    }

    #endregion Actors
}
