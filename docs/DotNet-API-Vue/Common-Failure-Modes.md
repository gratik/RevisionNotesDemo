# Common Failure Modes

> Subject: [DotNet-API-Vue](../README.md)

## Common Failure Modes

| Failure mode | Root cause | Prevention |
| --- | --- | --- |
| API logic scattered across components | No service/composable layer | Centralized API service + composables |
| Inconsistent auth behavior | Token handling duplicated | Request interceptor policy |
| Unexpected 400s on paging | Missing backend parameter guardrails | Validate query parameters and return clear validation details |
| Environment-specific integration bugs | Different local/prod routing assumptions | Vite proxy in dev + explicit production API base config |


