# Scenario

> Subject: [End-to-End-Case-Study](../README.md)

## Scenario

Feature: `PlaceOrder`

- Customer submits order with idempotency key
- Service validates business invariants
- Order is persisted with outbox event
- Downstream workflows are triggered asynchronously
- Deployment includes health gates and rollback path

---


