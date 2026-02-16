# Blazor

> Subject: [Front-End-DotNet-UI](../README.md)

## Blazor

### Strengths

- Component-based UI in C#
- Multiple render modes (SSR, Server, WebAssembly)
- Reusable components across web and hybrid

### Good vs Bad

```razor
@page "/report"

<!-- BAD: Synchronous work blocks rendering -->
@code {
    protected override void OnInitialized()
    {
        Thread.Sleep(5000);
    }
}
```

```razor
@page "/report"

@if (isLoading)
{
    <p>Loading...</p>
}
else
{
    <ReportTable Items="items" />
}

@code {
    private bool isLoading = true;
    private IReadOnlyList<ReportRow> items = Array.Empty<ReportRow>();

    protected override async Task OnInitializedAsync()
    {
        items = await service.GetRowsAsync();
        isLoading = false;
    }
}
```

### Pitfalls

- Heavy JS interop or unnecessary client-side state
- Large WebAssembly payloads without trimming

### Validation Examples

Blazor validation should use `EditForm` with `DataAnnotationsValidator` and display summaries.
See `BadValidation` and `GoodValidation` in [Learning/FrontEnd/BlazorUiExamples.cs](../../Learning/FrontEnd/BlazorUiExamples.cs).

---



