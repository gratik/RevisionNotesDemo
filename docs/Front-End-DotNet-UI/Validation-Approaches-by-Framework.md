# Validation Approaches by Framework

> Subject: [Front-End-DotNet-UI](../README.md)

## Validation Approaches by Framework

| Framework   | Primary Mechanism                                      | Typical UI Feedback                       |
| ----------- | ------------------------------------------------------ | ----------------------------------------- |
| MVC         | Data annotations + `ModelState`                        | `asp-validation-for` tag helpers          |
| Razor Pages | Data annotations + `ModelState` in handlers            | `asp-validation-for` tag helpers          |
| Blazor      | `EditForm` + `DataAnnotationsValidator`                | `ValidationSummary` + `ValidationMessage` |
| .NET MAUI   | MVVM validation + `TryParse`/custom errors             | Bound error text + visual states          |
| WPF         | `ValidationRule`/`IDataErrorInfo`                      | Error template + tooltip                  |
| WinForms    | `TryParse` + `ErrorProvider`                           | ErrorProvider icon + message              |
| Web Forms   | `RequiredFieldValidator`, `RegularExpressionValidator` | ValidationSummary + validator text        |

---


