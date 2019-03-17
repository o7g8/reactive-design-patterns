# Pattern Pipes and Filters

Pattern from the book of Vaughn Vernon "Reactive Messaging Patterns with the Actor Model".

A message is sent through a pipe of following actors:

* `OrderAcceptanceEndpoint`
* `Decrypter`
* `Authenticator`
* `Deduplicator`
* `OrderManagementSystem`

The original Scala example is <https://github.com/VaughnVernon/ReactiveMessagingPatterns_ActorModel/blob/master/src/co/vaughnvernon/reactiveenterprise/pipesandfilters/PipesAndFilters.scala>