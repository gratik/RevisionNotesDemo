# Common Failure Modes

> Subject: [DotNet-API-React](../README.md)

## Common Failure Modes

| Failure mode | Root cause | Prevention |
| --- | --- | --- |
| Random CORS errors | Middleware order or missing origin | Explicit policy + correct middleware order |
| Inconsistent errors in UI | Mixed backend error format | Standard `ProblemDetails` envelope |
| Memory leaks/warnings in React | Uncanceled async requests | `AbortController` in effect cleanup |
| Slow list pages | Returning full entities | API-side projection + paging DTO |


