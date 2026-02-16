// ==============================================================================
// FRONT-END UI - VUE + .NET API INTEGRATION
// ==============================================================================
// WHAT IS THIS?
// -------------
// Common patterns for connecting a Vue SPA to an ASP.NET Core API.
//
// WHY IT MATTERS
// --------------
// ✅ Encourages predictable state and API interaction boundaries
// ✅ Reduces duplicated request logic and fragile error handling
//
// WHEN TO USE
// -----------
// ✅ Vue SPA with independent .NET API service
// ✅ Apps needing composables/store-driven state management
//
// WHEN NOT TO USE
// ---------------
// ❌ Pure server-rendered pages where SPA complexity is not justified
// ❌ Teams without capacity to maintain API contract evolution
//
// REAL-WORLD EXAMPLE
// ------------------
// Axios instance + interceptor + typed view-model mapping for table pages.
// ==============================================================================

namespace RevisionNotesDemo.FrontEnd;

/// <summary>
/// Illustrative Vue + .NET API snippets showing good vs bad patterns.
/// </summary>
public static class VueApiIntegrationExamples
{
    public static void RunDemo()
    {
        Console.WriteLine("Vue + .NET API integration examples are illustrative only.");
        Console.WriteLine("See docs/DotNet-API-Vue.md for the full guide.");
    }

    /// <summary>
    /// BAD: API calls inside components with duplicated error logic.
    /// </summary>
    private const string BadVueComponent = @"<script setup>
import { ref, onMounted } from 'vue';
const products = ref([]);

onMounted(async () => {
  const res = await fetch('https://localhost:5001/api/products');
  products.value = await res.json();
});
</script>";

    /// <summary>
    /// GOOD: Shared axios client with auth and error normalization.
    /// </summary>
    private const string GoodVueApiClient = @"import axios from 'axios';

export const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  timeout: 8000,
  headers: { Accept: 'application/json' },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('access_token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    const title = error?.response?.data?.title ?? 'Request failed';
    const status = error?.response?.status ?? 0;
    return Promise.reject(new Error(`${title} (${status})`));
  }
);";

    /// <summary>
    /// GOOD: Vue composable isolates loading, error, and cancellation concerns.
    /// </summary>
    private const string GoodVueComposable = @"import { ref, onMounted, onUnmounted } from 'vue';
import { api } from './apiClient';

export function useProducts() {
  const products = ref([]);
  const isLoading = ref(true);
  const error = ref(null);
  const controller = new AbortController();

  onMounted(async () => {
    try {
      const res = await api.get('/api/products', { signal: controller.signal });
      products.value = res.data;
    } catch (err) {
      if (err.name !== 'CanceledError') error.value = err.message;
    } finally {
      isLoading.value = false;
    }
  });

  onUnmounted(() => controller.abort());
  return { products, isLoading, error };
}";

    /// <summary>
    /// GOOD: .NET API endpoint designed for SPA list rendering and filtering.
    /// </summary>
    private const string GoodApiEndpoint = @"[HttpGet]
public async Task<ActionResult<IReadOnlyList<ProductListItemDto>>> Search(
    [FromQuery] string? q,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 25,
    CancellationToken ct = default)
{
    if (page <= 0 || pageSize is < 1 or > 100)
    {
        return ValidationProblem(new Dictionary<string, string[]>
        {
            [""pagination""] = new[] { ""Invalid page or pageSize."" }
        });
    }

    var result = await _service.SearchAsync(q, page, pageSize, ct);
    return Ok(result);
}";

    /// <summary>
    /// GOOD: Vite proxy for local dev to avoid CORS drift between environments.
    /// </summary>
    private const string GoodViteProxy = @"import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';

export default defineConfig({
  plugins: [vue()],
  server: {
    proxy: {
      '/api': {
        target: 'https://localhost:5001',
        changeOrigin: true,
        secure: false,
      },
    },
  },
});";

    /// <summary>
    /// GOOD: API security posture for enterprise SPA integrations.
    /// </summary>
    private const string GoodApiSecurityPosture = @"builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration[""Auth:Authority""];
        options.Audience = ""products-api"";
        options.RequireHttpsMetadata = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(""products.read"", p => p.RequireClaim(""scope"", ""products.read""));
});

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(""api"", limiter =>
    {
        limiter.Window = TimeSpan.FromMinutes(1);
        limiter.PermitLimit = 100;
        limiter.QueueLimit = 0;
    });
});

app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(""SpaPolicy"");
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();";

    /// <summary>
    /// GOOD: Structured logs and trace context in API + UI for incident triage.
    /// </summary>
    private const string GoodLoggingAndTracing = @"builder.Services.AddOpenTelemetry()
    .WithTracing(t => t
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation())
    .WithMetrics(m => m
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation());

logger.LogInformation(""Products search executed. Page={Page} Size={PageSize}"", page, pageSize);
// Avoid logging bearer tokens, passwords, or full PII payloads.";

    /// <summary>
    /// GOOD: Vue client sends correlation metadata and logs bounded diagnostics.
    /// </summary>
    private const string GoodVueTelemetry = @"const traceId = crypto.randomUUID();

const response = await api.get('/api/products', {
  headers: { 'X-Trace-Id': traceId },
});

telemetry.trackEvent('products_loaded', { traceId, count: response.data.length });
telemetry.trackError('products_load_failed', { traceId, code: 'PRODUCTS_LOAD' });";
}
