# Performance Regression Checks

## Guardrail approach

- Track baseline p95/p99 latency and throughput.
- Add repeatable benchmark/load checks in CI or pre-release gates.
- Alert on meaningful regression thresholds.
- Tie performance checks to release decisions, not ad-hoc reviews.
