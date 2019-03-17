using System;
using Xunit;
using pattern_pipes_and_filters;
using Akka.Actor;

namespace pattern_pipes_and_filters.test
{
    public class UnitTest : Akka.TestKit.Xunit.TestKit {
        [Fact]
        public void Authenticator_NoCertificate()
        {
            var probe = CreateTestProbe();
            var actor = Sys.ActorOf(Props.Create(() => new Authenticator(probe)));
            
            actor.Tell(new ProcessIncomingOrder("MyOrder"));
            probe.ExpectMsgFrom<ProcessIncomingOrder>(actor, msg => msg.OrderText == "MyOrder");
        }

        [Fact]
        public void Authenticator_WithCertificate()
        {
            var probe = CreateTestProbe();
            var actor = Sys.ActorOf(Props.Create(() => new Authenticator(probe)));
            
            actor.Tell(new ProcessIncomingOrder("(certificate)MyOrder"));
            probe.ExpectMsgFrom<ProcessIncomingOrder>(actor, msg => msg.OrderText == "MyOrder");
        }
    }
}
