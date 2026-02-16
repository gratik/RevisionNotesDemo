# Frontend Patterns (Vue)

> Subject: [DotNet-API-Vue](../README.md)

## Frontend Patterns (Vue)

### Shared axios client

```ts
export const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  timeout: 8000,
  headers: { Accept: "application/json" },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem("access_token");
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});
```

### Composable for list page

```ts
export function useProducts() {
  const products = ref<ProductListItemDto[]>([]);
  const isLoading = ref(true);
  const error = ref<string | null>(null);
  const controller = new AbortController();

  onMounted(async () => {
    try {
      const response = await api.get("/api/products", { signal: controller.signal });
      products.value = response.data;
    } catch (err: any) {
      if (err.name !== "CanceledError") error.value = err.message ?? "Request failed";
    } finally {
      isLoading.value = false;
    }
  });

  onUnmounted(() => controller.abort());
  return { products, isLoading, error };
}
```

### Vite local proxy

```ts
export default defineConfig({
  server: {
    proxy: {
      "/api": {
        target: "https://localhost:5001",
        changeOrigin: true,
        secure: false,
      },
    },
  },
});
```


