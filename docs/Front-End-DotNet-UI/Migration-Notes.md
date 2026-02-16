# Migration Notes

> Subject: [Front-End-DotNet-UI](../README.md)

## Migration Notes

- **Web Forms -> MVC/Razor Pages**: Move server controls to tag helpers, replace view state with explicit models, and use data annotations + `ModelState`.
- **WinForms -> WPF**: Keep business logic, move UI to XAML + MVVM, and use `ValidationRule` or `IDataErrorInfo` for input.
- **WPF/WinForms -> .NET MAUI**: Reuse view models, rewrite views in MAUI XAML, and validate in the VM with user-friendly errors.
- **MVC -> Blazor**: Promote view models to component parameters and shift UI logic into components; keep server validation on APIs.

---


