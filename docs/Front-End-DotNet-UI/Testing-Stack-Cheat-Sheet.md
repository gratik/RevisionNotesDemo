# Testing Stack Cheat-Sheet

> Subject: [Front-End-DotNet-UI](../README.md)

## Testing Stack Cheat-Sheet

| Framework   | Unit Tests                       | UI/Integration Tests                    |
| ----------- | -------------------------------- | --------------------------------------- |
| MVC         | xUnit + controller/service tests | Playwright (web) or Selenium            |
| Razor Pages | xUnit + PageModel tests          | Playwright (web) or Selenium            |
| Blazor      | bUnit for components + xUnit     | Playwright (Web) / MAUI UITest (Hybrid) |
| .NET MAUI   | xUnit + VM tests                 | Appium or MAUI UITest (if available)    |
| WPF         | xUnit + VM tests                 | FlaUI or WinAppDriver                   |
| WinForms    | xUnit + presenter tests          | FlaUI or WinAppDriver                   |
| Web Forms   | xUnit + utility tests            | Selenium (legacy web UI)                |

---


