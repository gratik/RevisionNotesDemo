# Extended Comparison

> Subject: [Front-End-DotNet-UI](../README.md)

## Extended Comparison

| Technology           | Startup/Perf              | Deployment Model              | Testability                  | Offline Support  | Accessibility Defaults       |
| -------------------- | ------------------------- | ----------------------------- | ---------------------------- | ---------------- | ---------------------------- |
| **ASP.NET Core MVC** | Fast server response      | Server app + static assets    | High (controllers, services) | N/A (server web) | Good with proper markup      |
| **Razor Pages**      | Fast server response      | Server app + static assets    | High (page models)           | N/A (server web) | Good with proper markup      |
| **Blazor Server**    | Low client payload        | Server app + SignalR circuit  | High (components + services) | Limited          | Good with component patterns |
| **Blazor WASM**      | Higher initial download   | Static site + API backend     | Medium (UI + integration)    | Possible         | Good with component patterns |
| **.NET MAUI**        | Native startup varies     | App store / MSIX / installers | Medium (UI + VM tests)       | Strong           | Depends on platform tooling  |
| **WPF**              | Strong on Windows         | MSI/MSIX/ClickOnce            | High (VM + UI automation)    | Strong           | Good with WPF automation     |
| **WinForms**         | Very fast on Windows      | MSI/MSIX/ClickOnce            | Medium (UI automation heavy) | Strong           | Depends on control usage     |
| **Web Forms**        | Server-bound + view state | IIS-hosted .NET Framework     | Low (legacy patterns)        | N/A (server web) | Mixed, often manual fixes    |

---


