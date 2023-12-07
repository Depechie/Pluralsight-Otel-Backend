# Pluralsight-Otel-Backend

Repository containing the demo projects used in my Pluralsight course [Observability with OpenTelemetry and Grafana](http://www.pluralsight.com/courses/opentelemetry-grafana-observability).

It consists out of 2 ASP.NET Web API services and 1 Service worker. The communication between the services is done with RabbitMQ, the Basket API will push a message to the queue and the Service worker will pick this up and process it.
The processing of the message will trigger 2 calls to the Catalog API.

Logging is done with SeriLog and it uses an OpenTelemetry Sink to export logs to an OpenTelemetry Collector.
Traces are collected using the OpenTelemetry auto instrumentation for .NET.

The infrastructure setup through docker can be found here [Pluralsight OTEL infrastructure](https://github.com/Depechie/Pluralsight-Otel-Infrastructure).

For ease of use, there is also following repository [https://github.com/Depechie/Pluralsight-Otel-Demo](https://github.com/Depechie/Pluralsight-Otel-Demo) which contains the Backend and Infrastructure, along with a docker compose file that will spin up everything together.
