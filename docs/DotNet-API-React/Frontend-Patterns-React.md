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


