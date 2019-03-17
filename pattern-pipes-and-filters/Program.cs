using System;
using Akka.Actor;

namespace pattern_pipes_and_filters
{
    class Program
    {
        static void Main(string[] args) {
            var orderText = "(encryption)(certificate)<order id='123'>...</order>";

            using(var actorSystem = ActorSystem.Create("pattern-pipes-and-filters")) {
                var filter5 = actorSystem.ActorOf<OrderManagementSystem>("orderManagementSystem");
                var filter4 = actorSystem.ActorOf(Deduplicator.Props(filter5), "deduplicator");
                var filter3 = actorSystem.ActorOf(Authenticator.Props(filter4), "authenticator");
                var filter2 = actorSystem.ActorOf(Decrypter.Props(filter3), "decrypter");
                var filter1 = actorSystem.ActorOf(OrderAcceptanceEndpoint.Props(filter2), "orderacceptanceendpoint");
                
                filter1.Tell(orderText);
                filter1.Tell(orderText);
                
                Console.Read();
            }
        }
    }
}
