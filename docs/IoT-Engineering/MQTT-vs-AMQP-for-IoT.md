# MQTT vs AMQP for IoT

## Decision guide

- Choose MQTT for constrained devices and unstable networks.
- Choose AMQP for richer broker semantics and enterprise workflows.
- Standardize retries, timeouts, and backoff independent of transport.
- Validate protocol choice with throughput and reconnect behavior tests.
