# Contract Testing for Messaging

## Why it matters

- Prevents producer-consumer drift across teams.
- Validates schema evolution before release.
- Reduces integration outages from payload changes.

## Baseline

- Version contracts.
- Run consumer-driven checks in CI.
- Block release on incompatible schema updates.
