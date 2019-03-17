# Conventions

_From the book "Reactive Messaging Patterns with the Actor Model"_

* Define a _Canonical Message Model_.

* Implement messages using immutable value types. Never mutate messages across actors.

* Place all messages supported by the given actor type in the source module along with the definition of the receiving actor. The approach works best when the system is built using only local actors.

* Place all common message types into one (Scala) source file. The approach works best when actor systems integrate and together define a Canonical Message Model.

