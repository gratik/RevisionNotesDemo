# Failure Playbook

> Subject: [Distributed-Consistency](../README.md)

## Failure Playbook

If duplicate effects occur:
1. Verify idempotency key propagation end-to-end.
2. Check inbox dedupe store retention and key hashing.
3. Validate compensation handlers are idempotent.
4. Inspect replay/retry policies in brokers and clients.

---


