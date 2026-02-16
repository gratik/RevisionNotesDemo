# Validation Checklist

> Subject: [DotNet-API-Vue](../README.md)

## Validation Checklist

- Vue components call composables/services, not raw API endpoints.
- Axios instance handles auth and normalized errors centrally.
- Backend validates pagination/filter inputs explicitly.
- ProblemDetails is enabled for consistent non-2xx responses.
- Local and production API base routing are documented and tested.
- Authorization policies and scopes are explicit and covered by tests.
- Logs are structured with correlation ids and sensitive data redaction.
- Dashboards include latency, error-rate, and dependency health views.


