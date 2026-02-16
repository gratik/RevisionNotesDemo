# Frontend Patterns (React)

> Subject: [DotNet-API-React](../README.md)

## Frontend Patterns (React)

### Shared API client

```ts
const API_BASE = import.meta.env.VITE_API_BASE_URL;

export async function apiGet(path: string, token?: string, signal?: AbortSignal) {
  const response = await fetch(`${API_BASE}${path}`, {
    headers: {
      Accept: "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    },
    signal,
  });

  if (!response.ok) {
    const details = await response.json().catch(() => ({ title: "Request failed", status: response.status }));
    throw new Error(`${details.title} (${details.status})`);
  }

  return await response.json();
}
```

### Hook with cancellation

```ts
export function useOrders(token?: string) {
  const [orders, setOrders] = useState<OrderDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const controller = new AbortController();
    setIsLoading(true);

    apiGet("/api/orders", token, controller.signal)
      .then(setOrders)
      .catch((err) => err.name !== "AbortError" && setError(err.message))
      .finally(() => setIsLoading(false));

    return () => controller.abort();
  }, [token]);

  return { orders, isLoading, error };
}
```

## Detailed Guidance

API guidance focuses on contract stability, secure input/output handling, and production-ready observability.

### Design Notes
- Define success criteria for Frontend Patterns (React) before implementation work begins.
- Keep boundaries explicit so Frontend Patterns (React) decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Frontend Patterns (React) in production-facing code.
- When performance, correctness, or maintainability depends on consistent Frontend Patterns (React) decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Frontend Patterns (React) as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Frontend Patterns (React) is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Frontend Patterns (React) are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

