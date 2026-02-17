# Edge Connectivity and Store-Forward

## Offline-tolerant patterns

- Buffer locally during disconnects with explicit capacity limits.
- Replay with ordering keys and deduplication IDs on reconnect.
- Define shedding policy when local storage is near exhaustion.
- Monitor replay backlog age and offline duration as incident signals.
