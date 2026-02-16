# Analyzer suppressions (`GlobalSuppressions.cs`)

> Subject: [Suppression-Audit](../README.md)

## Analyzer suppressions (`GlobalSuppressions.cs`)

Source: `Properties/GlobalSuppressions.cs`

| Rule ID | Scope/Target | Justification summary | Review owner | Review cadence |
| --- | --- | --- | --- | --- |
| `CA1848` | `RevisionNotesDemo.Logging` | Demonstrates baseline logging APIs before optimized delegate patterns. | Maintainers | Quarterly |
| `CA1707` | `RevisionNotesDemo.Performance` | Uses underscore naming for side-by-side benchmark readability. | Maintainers | Quarterly |
| `CA1707` | `RevisionNotesDemo.DataAccess` | Uses underscore labels to distinguish anti-pattern vs improved variants. | Maintainers | Quarterly |
| `CA1707` | `RevisionNotesDemo.DesignPatterns.Behavioral` | Uses underscore suffixes to separate contrasting variants. | Maintainers | Quarterly |
| `CA1707` | `RevisionNotesDemo.Logging` | Uses test-style naming to match scenario-focused examples. | Maintainers | Quarterly |
| `CA1707` | `RevisionNotesDemo.WebAPI.Versioning` | Uses scenario labels for demo readability. | Maintainers | Quarterly |
| `CA1711` | `RevisionNotesDemo` | Keeps conventional suffixes (`Queue`, `Check`, `Event`) for discoverability. | Maintainers | Quarterly |
| `CA1716` | `RevisionNotesDemo` | Mirrors common API naming patterns used in tutorials. | Maintainers | Quarterly |
| `CA1710` | `RevisionNotesDemo` | Preserves concise names used across teaching samples. | Maintainers | Quarterly |
| `CA1805` | `RevisionNotesDemo` | Keeps explicit defaults for pedagogy. | Maintainers | Quarterly |
| `CA1859` | `RevisionNotesDemo` | Demonstrates interface-first abstraction boundaries. | Maintainers | Quarterly |
| `CA1840` | `RevisionNotesDemo` | Uses `Thread.CurrentThread` intentionally in thread concept demos. | Maintainers | Quarterly |
| `CA1860` | `RevisionNotesDemo` | Prefers LINQ readability in learning-focused samples. | Maintainers | Quarterly |
| `CA1861` | `RevisionNotesDemo` | Keeps inline literals for concise teaching examples. | Maintainers | Quarterly |
| `CA1825` | `RevisionNotesDemo` | Shows explicit array patterns intentionally in some snippets. | Maintainers | Quarterly |
| `CA1847` | `RevisionNotesDemo` | Keeps string-literal forms to match narrative examples. | Maintainers | Quarterly |
| `CA1866` | `RevisionNotesDemo` | Varies overload usage intentionally for API teaching. | Maintainers | Quarterly |
| `CA1845` | `RevisionNotesDemo` | Prioritizes readability over span-level micro-optimization in baseline demos. | Maintainers | Quarterly |
| `CA1829` | `RevisionNotesDemo` | Uses query-style operators intentionally in LINQ-focused sections. | Maintainers | Quarterly |
| `CA1051` | `RevisionNotesDemo` | Some DTO/sample types expose fields for brevity. | Maintainers | Quarterly |
| `CA1000` | `RevisionNotesDemo` | Generic static-member behavior is demonstrated intentionally. | Maintainers | Quarterly |
| `CA1510` | `RevisionNotesDemo` | Includes classic guard clauses for comparison with modern alternatives. | Maintainers | Quarterly |
| `CA2012` | `RevisionNotesDemo` | Includes ValueTask misuse examples to teach pitfalls. | Maintainers | Quarterly |
| `CA5350` | `RevisionNotesDemo.Security` | Uses HMAC-SHA1 in TOTP RFC-compatibility examples. | Maintainers | Quarterly |
| `ASPDEPR002` | `RevisionNotesDemo.WebAPI` | Includes deprecated ASP.NET APIs in migration-oriented examples. | Maintainers | Quarterly |


