# Project-level `NoWarn` entries

> Subject: [Suppression-Audit](../README.md)

## Project-level `NoWarn` entries

Source: `Directory.Build.props`

| Rule ID | Scope | Justification | Review owner | Review cadence |
| --- | --- | --- | --- | --- |
| `CS0219` | All projects under repo | Some demos intentionally keep assigned-but-unused locals to show intermediate states in teaching flows. | Maintainers | Quarterly |
| `CS0414` | All projects under repo | Certain class-level fields are intentionally retained to explain state transitions and contrasts. | Maintainers | Quarterly |
| `CS0168` | All projects under repo | Some examples intentionally declare exception placeholders while discussing error handling evolution. | Maintainers | Quarterly |
| `CS0169` | All projects under repo | Some demos include placeholder fields to compare design alternatives. | Maintainers | Quarterly |
| `CS8763` | All projects under repo | Nullable-flow edge cases are intentionally demonstrated in specific educational snippets. | Maintainers | Quarterly |
| `ASPDEPR002` | Web API educational paths | Migration examples intentionally include deprecated API usage for contrast. | Maintainers | Quarterly |
| `IDE0005` | All projects under repo | Teaching snippets preserve imports in places where examples are copy/paste friendly across sections. | Maintainers | Quarterly |


