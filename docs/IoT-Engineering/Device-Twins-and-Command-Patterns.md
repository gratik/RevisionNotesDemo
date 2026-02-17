# Device Twins and Command Patterns

## Reliable configuration and command control

- Use desired/reported properties for eventual convergence and state visibility.
- Version twin payloads to avoid incompatible rollouts.
- Use direct methods for bounded commands with clear timeout/retry behavior.
- Require idempotent device-side command handling.
