# ASP.NET Web Forms

> Subject: [Front-End-DotNet-UI](../README.md)

## ASP.NET Web Forms

### Strengths

- Event-driven model
- Rich server control library
- Familiar for legacy apps

### Good vs Bad

```aspx
<!-- BAD: View state bloat -->
<asp:GridView ID="Grid1" runat="server" EnableViewState="true">
  <!-- Complex row events and heavy view state -->
</asp:GridView>
```

```aspx
<!-- GOOD: Disable view state where possible -->
<asp:GridView ID="Grid1" runat="server" EnableViewState="false" />
```

### Pitfalls

- Large view state payloads
- Limited modern tooling compared to ASP.NET Core

### Validation Examples

Web Forms validation should use server validators instead of raw `Request` access.
See `BadValidation` and `GoodValidation` in [Learning/FrontEnd/WebFormsUiExamples.cs](../../Learning/FrontEnd/WebFormsUiExamples.cs).

---



