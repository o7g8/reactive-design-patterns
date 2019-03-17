# Getting Started

[Akka in .Net Core – Part 1 (Creating an Akka Actor)](https://dotnetcorecentral.com/blog/akka-in-net-core-part-1-creating-an-akka-actor/)

[Designing Akka.NET Applications from Scratch Part 1: Go with the Flow](https://petabridge.com/blog/akkadotnet-application-design-part1/)

[Zoran Maksimovic - Akka.NET Succinctly](https://www.syncfusion.com/ebooks/akka_net_succinctly)

## Facts about AKKA.NET

[Message Delivery Reliability](https://getakka.net/articles/concepts/message-delivery-reliability.html)

[Akka Message Delivery - At-Most-Once, At-Least-Once, and Exactly-Once - Part 1 At-Most-Once](https://developer.lightbend.com/blog/2017-08-10-atotm-akka-messaging-part-1/index.html)

[How to Guarantee Delivery of Messages in Akka.NET](https://petabridge.com/blog/akkadotnet-at-least-once-message-delivery/):
> All in-memory message passing within one application, for instance, is going to be guaranteed unless you hit an OutOfMemoryException or any of the other standard CLR failures - those failure cases are no different than what would happen if you were invoking methods instead of passing messages.

You need guaranteed delivery (only) when you’re passing messages over the network.

[Why You Should Try to Avoid Exactly Once Message Delivery](https://petabridge.com/blog/akkadotnet-exactly-once-message-delivery/): use idempotent data structures (to allow for duplicate messages) in the at-least-once systems.


## Working with Actors

[Working with Actors](https://www.syncfusion.com/ebooks/akka_net_succinctly/working-with-actors)

Use async/await only inside of `ReceiveAsync<T>`.


## Unit Testing

[How to Unit Test Akka.NET Actors with Akka.TestKit](https://petabridge.com/blog/how-to-unit-test-akkadotnet-actors-akka-testkit/)

[Testing Actor Systems](https://getakka.net/articles/actors/testing-actor-systems.html)

[Unit Testing Akka.NET](https://www.syncfusion.com/ebooks/akka_net_succinctly/unit-testing-akka-net)

## Misc

[Logging](https://getakka.net/articles/utilities/logging.html)

[Circuit Breaker](https://getakka.net/articles/utilities/circuit-breaker.html)