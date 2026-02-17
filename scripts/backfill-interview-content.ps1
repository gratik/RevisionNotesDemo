param(
    [Parameter(Mandatory = $true)]
    [string[]]$Modules
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$changedListPath = Join-Path $repoRoot "tmp_modified_docs.txt"

if (-not (Test-Path $changedListPath)) {
    throw "Missing file list: $changedListPath"
}

$contextByModule = @{
    "Core-CSharp" = @{
        Focus = "core C# language features and API design"
        Why = "it directly affects correctness, readability, and maintainability"
        UseCase = "designing reusable domain and application abstractions"
        Tradeoff = "flexibility vs API complexity"
        Pitfall = "over-abstracting simple code paths; keep public contracts intentional"
        Prereq = "Core C# syntax, object-oriented fundamentals, and basic collection usage."
    }
    "Advanced-CSharp" = @{
        Focus = "runtime and advanced type-system behavior in C#"
        Why = "it helps solve specialized problems without sacrificing reliability"
        UseCase = "building framework-like components and diagnostics tooling"
        Tradeoff = "powerful features vs maintainability for less-experienced maintainers"
        Pitfall = "using advanced mechanisms where straightforward code would be clearer"
        Prereq = "Strong Core C# fundamentals and familiarity with reflection, delegates, and generics."
    }
    "Modern-CSharp" = @{
        Focus = "newer C# language capabilities"
        Why = "modern syntax reduces boilerplate and improves intent clarity"
        UseCase = "updating legacy code to safer and more expressive patterns"
        Tradeoff = "new language features vs team familiarity and consistency"
        Pitfall = "mixing old and new idioms inconsistently across the same codebase"
        Prereq = "Core C# concepts, nullable awareness, and common refactoring patterns."
    }
    "OOP-Principles" = @{
        Focus = "object-oriented design boundaries and responsibilities"
        Why = "good boundaries reduce coupling and improve testability"
        UseCase = "designing services and entities with clear responsibilities"
        Tradeoff = "extensibility vs added abstraction layers"
        Pitfall = "applying principles mechanically without considering domain context"
        Prereq = "Classes/interfaces, dependency inversion basics, and unit testing fundamentals."
    }
    "DotNet-Concepts" = @{
        Focus = ".NET platform and dependency injection fundamentals"
        Why = "these concepts determine startup wiring and runtime behavior"
        UseCase = "configuring robust service registration and app composition"
        Tradeoff = "centralized container control vs over-reliance on DI magic"
        Pitfall = "lifetime mismatches causing subtle runtime bugs"
        Prereq = "Basic ASP.NET Core app structure and service registration syntax."
    }
    "LINQ-Queries" = @{
        Focus = "query composition over in-memory and provider-backed data"
        Why = "correct query shape impacts both readability and performance"
        UseCase = "implementing filtering, projection, and aggregation in business workflows"
        Tradeoff = "concise query syntax vs hidden complexity in execution behavior"
        Pitfall = "accidental multiple enumeration or provider-side translation surprises"
        Prereq = "Collections, lambdas, and deferred execution basics."
    }
    "Memory-Management" = @{
        Focus = "allocation, lifetime, and garbage collection behavior"
        Why = "memory patterns directly affect latency, throughput, and stability"
        UseCase = "reducing allocation pressure in hot execution paths"
        Tradeoff = "micro-optimizations vs maintainable code"
        Pitfall = "premature optimization without profiling evidence"
        Prereq = "Value/reference type basics and runtime execution model familiarity."
    }
    "Async-Multithreading" = @{
        Focus = "concurrency and asynchronous flow control"
        Why = "it determines responsiveness and resource efficiency under load"
        UseCase = "handling I/O workloads safely in APIs and background jobs"
        Tradeoff = "parallelism gains vs coordination complexity"
        Pitfall = "blocking async code paths and causing deadlocks or thread starvation"
        Prereq = "Tasks/async-await basics and thread-safety fundamentals."
    }
    "API-Documentation" = @{
        Focus = "API contract clarity and discoverability"
        Why = "clear docs reduce integration defects and support overhead"
        UseCase = "aligning backend changes with consumer expectations"
        Tradeoff = "detailed documentation vs ongoing maintenance overhead"
        Pitfall = "docs drifting from real runtime behavior"
        Prereq = "HTTP API fundamentals, OpenAPI basics, and endpoint versioning awareness."
    }
    "Web-API-MVC" = @{
        Focus = "ASP.NET endpoint architecture patterns"
        Why = "architecture choices affect testability, throughput, and maintainability"
        UseCase = "selecting minimal API, controller API, or MVC by problem shape"
        Tradeoff = "developer speed vs explicit control and extensibility"
        Pitfall = "mixing styles without clear boundaries"
        Prereq = "ASP.NET Core request pipeline and routing fundamentals."
    }
    "gRPC" = @{
        Focus = "contract-first RPC communication"
        Why = "it optimizes service-to-service communication and typed contracts"
        UseCase = "high-throughput internal service calls with strict schemas"
        Tradeoff = "performance and typing vs ecosystem/browser constraints"
        Pitfall = "choosing gRPC for scenarios where REST interoperability is required"
        Prereq = "HTTP/2 basics, protobuf awareness, and service contract versioning."
    }
    "Entity-Framework" = @{
        Focus = "ORM-based data modeling and persistence"
        Why = "query shape and tracking behavior strongly affect performance"
        UseCase = "building data access layers with maintainable domain mappings"
        Tradeoff = "developer productivity vs query/control precision"
        Pitfall = "N+1 queries and incorrect tracking strategy"
        Prereq = "Relational data modeling and basic LINQ provider behavior."
    }
    "Data-Access" = @{
        Focus = "data persistence and query strategy selection"
        Why = "it drives correctness, latency, and transactional integrity"
        UseCase = "choosing between EF Core, Dapper, and ADO.NET by workload"
        Tradeoff = "abstraction convenience vs explicit SQL control"
        Pitfall = "ignoring transaction and indexing behavior in production paths"
        Prereq = "SQL fundamentals, connection lifecycle, and transaction basics."
    }
    "DotNet-API-React" = @{
        Focus = "backend/frontend integration design for React clients"
        Why = "contract and state decisions affect delivery speed and reliability"
        UseCase = "building resilient API surfaces consumed by React applications"
        Tradeoff = "rich backend contracts vs frontend adaptability"
        Pitfall = "inconsistent API error/validation contracts across endpoints"
        Prereq = "REST API basics and React data-fetching/state management familiarity."
    }
    "DotNet-API-Vue" = @{
        Focus = "backend/frontend integration design for Vue clients"
        Why = "consistent contracts reduce frontend complexity and defects"
        UseCase = "implementing API patterns that scale with Vue feature growth"
        Tradeoff = "strict contracts vs rapid iteration flexibility"
        Pitfall = "frontend workarounds due to ambiguous backend conventions"
        Prereq = "REST API patterns and Vue component/state fundamentals."
    }
    "Message-Architecture" = @{
        Focus = "asynchronous messaging and event-driven coordination"
        Why = "it improves decoupling and throughput in distributed systems"
        UseCase = "reliable integration between independently evolving services"
        Tradeoff = "scalability and decoupling vs operational complexity"
        Pitfall = "underestimating retries, ordering, and idempotency concerns"
        Prereq = "Distributed systems basics, queues/topics, and eventual consistency concepts."
    }
    "Azure-Hosting" = @{
        Focus = "Azure deployment and service composition decisions"
        Why = "hosting choices determine cost, resilience, and operations burden"
        UseCase = "mapping workloads to the right Azure compute and messaging services"
        Tradeoff = "managed-service simplicity vs workload-specific customization"
        Pitfall = "optimizing for feature set without operational cost modeling"
        Prereq = "Cloud deployment basics and core Azure service familiarity."
    }
    "Deployment-DevOps" = @{
        Focus = "delivery automation and runtime operational practices"
        Why = "pipeline quality determines release safety and iteration speed"
        UseCase = "building repeatable CI/CD with rollout safeguards"
        Tradeoff = "deployment velocity vs risk controls and verification depth"
        Pitfall = "shipping without rollback and observability guardrails"
        Prereq = "Git workflows, CI/CD concepts, and container/runtime basics."
    }
    "Security" = @{
        Focus = "application and API security controls"
        Why = "security failures are high-impact and expensive to remediate"
        UseCase = "implementing defense-in-depth across authentication and authorization"
        Tradeoff = "security strictness vs developer and user ergonomics"
        Pitfall = "treating security as a one-time feature instead of an ongoing practice"
        Prereq = "AuthN/AuthZ basics, secret management, and secure coding fundamentals."
    }
    "Resilience" = @{
        Focus = "fault handling and recovery design"
        Why = "resilience patterns preserve service quality during failures"
        UseCase = "protecting dependencies with retries, circuit breakers, and timeouts"
        Tradeoff = "improved availability vs added control-flow complexity"
        Pitfall = "stacking policies without understanding interaction effects"
        Prereq = "Distributed call patterns, timeout semantics, and transient fault basics."
    }
    "RealTime" = @{
        Focus = "stateful real-time communication patterns"
        Why = "real-time paths amplify scale and connection-lifecycle concerns"
        UseCase = "broadcasting live updates to connected clients safely"
        Tradeoff = "low-latency delivery vs connection/session management overhead"
        Pitfall = "assuming connection permanence and ignoring reconnection flows"
        Prereq = "WebSocket/SignalR basics, auth context propagation, and scaling fundamentals."
    }
    "HealthChecks" = @{
        Focus = "service health signaling and readiness strategy"
        Why = "accurate health endpoints prevent bad routing and noisy incidents"
        UseCase = "separating liveness/readiness/dependency checks by purpose"
        Tradeoff = "signal depth vs probe execution overhead"
        Pitfall = "health checks that are either too shallow or too expensive"
        Prereq = "ASP.NET middleware basics and service dependency mapping."
    }
    "Logging-Observability" = @{
        Focus = "telemetry design for diagnostics and operations"
        Why = "good observability shortens detection and recovery times"
        UseCase = "correlating logs, traces, and metrics across service boundaries"
        Tradeoff = "high-cardinality detail vs telemetry cost/noise"
        Pitfall = "missing correlation context during incident response"
        Prereq = "Logging basics, distributed tracing concepts, and monitoring fundamentals."
    }
    "Performance" = @{
        Focus = "throughput and latency optimization in .NET workloads"
        Why = "performance bottlenecks directly impact user experience and cost"
        UseCase = "profiling and tuning high-traffic endpoints or background workers"
        Tradeoff = "raw speed improvements vs code clarity and maintenance cost"
        Pitfall = "optimizing without measuring baseline and regression impact"
        Prereq = "Profiling basics, memory allocation awareness, and async flow fundamentals."
    }
    "Operational-Excellence" = @{
        Focus = "incident response and service reliability operations"
        Why = "operational rigor keeps systems stable under real production pressure"
        UseCase = "defining SLOs, runbooks, and escalation practices"
        Tradeoff = "process rigor vs delivery overhead"
        Pitfall = "unclear ownership and playbooks during incidents"
        Prereq = "Production support basics, monitoring, and on-call fundamentals."
    }
    "Build-Warning-Triage" = @{
        Focus = "build quality and warning governance"
        Why = "warning debt compounds and hides real regressions"
        UseCase = "establishing warning policies that teams can sustain"
        Tradeoff = "strict quality gates vs short-term delivery pressure"
        Pitfall = "blanket suppression without triage rationale"
        Prereq = "Compiler/analyzer warning basics and CI policy familiarity."
    }
    "Design-Patterns" = @{
        Focus = "reusable design solutions for recurring software problems"
        Why = "pattern choice shapes long-term extensibility and readability"
        UseCase = "selecting pattern structure to simplify complex behavior"
        Tradeoff = "architectural consistency vs accidental overengineering"
        Pitfall = "forcing patterns where straightforward code is enough"
        Prereq = "Object-oriented design fundamentals and refactoring familiarity."
    }
    "Domain-Driven-Design" = @{
        Focus = "domain modeling and bounded-context design"
        Why = "domain boundaries reduce ambiguity and integration friction"
        UseCase = "mapping business language into explicit aggregates and workflows"
        Tradeoff = "model purity vs practical delivery constraints"
        Pitfall = "anemic models and leaky bounded-context boundaries"
        Prereq = "Core domain modeling concepts and layered architecture familiarity."
    }
    "Distributed-Consistency" = @{
        Focus = "consistency strategy across distributed systems"
        Why = "consistency decisions determine correctness during partial failures"
        UseCase = "orchestrating workflows with idempotency and compensations"
        Tradeoff = "strong consistency guarantees vs availability/latency"
        Pitfall = "assuming exactly-once semantics from transport alone"
        Prereq = "Distributed systems failure modes and messaging fundamentals."
    }
    "End-to-End-Case-Study" = @{
        Focus = "holistic architecture and delivery decision-making"
        Why = "end-to-end framing exposes cross-cutting tradeoffs"
        UseCase = "walking from requirements to production-ready implementation choices"
        Tradeoff = "completeness vs complexity and delivery time"
        Pitfall = "solving components in isolation without system-level constraints"
        Prereq = "Working familiarity with API, data, observability, and deployment basics."
    }
    "Engineering-Process" = @{
        Focus = "team practices for predictable delivery quality"
        Why = "process quality affects defect rates and delivery flow"
        UseCase = "improving review, planning, and definition-of-done consistency"
        Tradeoff = "governance structure vs execution speed"
        Pitfall = "process checklists with weak ownership and feedback loops"
        Prereq = "Version control workflow, code review basics, and agile delivery fundamentals."
    }
    "Practical-Patterns" = @{
        Focus = "high-value implementation patterns for day-to-day engineering"
        Why = "practical patterns reduce repeated design mistakes"
        UseCase = "standardizing common cross-cutting behaviors in services"
        Tradeoff = "pattern reuse vs context-specific customization needs"
        Pitfall = "copying patterns without validating fit for the current problem"
        Prereq = "Core API/service development skills and dependency injection familiarity."
    }
    "Runtime-Section-Map" = @{
        Focus = "navigation and structure mapping across runtime topics"
        Why = "clear map pages speed onboarding and content discovery"
        UseCase = "linking related runtime concepts for targeted study plans"
        Tradeoff = "high-level mapping vs depth in each referenced section"
        Pitfall = "stale maps that no longer reflect actual content structure"
        Prereq = "Familiarity with the docs index and runtime-focused module overviews."
    }
    "Suppression-Audit" = @{
        Focus = "governance of warning and analyzer suppressions"
        Why = "controlled suppressions protect long-term code quality"
        UseCase = "auditing suppression reasons and expiration policies"
        Tradeoff = "short-term unblock vs long-term maintenance debt"
        Pitfall = "permanent suppressions with no review cadence"
        Prereq = "Analyzer warnings, code quality policy, and CI enforcement basics."
    }
    "Front-End-DotNet-UI" = @{
        Focus = ".NET UI stack patterns and frontend integration choices"
        Why = "UI architecture affects usability, testability, and delivery speed"
        UseCase = "choosing the right .NET UI approach for product constraints"
        Tradeoff = "rapid UI iteration vs maintainable component structure"
        Pitfall = "tight coupling between UI and data access concerns"
        Prereq = "Frontend fundamentals and basic .NET web/UI application structure."
    }
    "IoT-Engineering" = @{
        Focus = "device-to-cloud architecture and telemetry processing"
        Why = "IoT systems require robust identity, ingestion, and reliability controls"
        UseCase = "designing secure telemetry pipelines at scale"
        Tradeoff = "device simplicity vs backend processing complexity"
        Pitfall = "weak device identity and offline handling strategy"
        Prereq = "Messaging fundamentals, cloud services basics, and event processing awareness."
    }
    "Interview-Preparation" = @{
        Focus = "communication structure for technical interviews"
        Why = "clear articulation of tradeoffs improves interview signal quality"
        UseCase = "translating implementation knowledge into concise answers"
        Tradeoff = "brevity vs sufficient technical depth"
        Pitfall = "memorized answers that ignore problem context"
        Prereq = "Comfort with core module topics and deliberate timed practice."
    }
    "Testing" = @{
        Focus = "verification strategies across unit, integration, and system levels"
        Why = "testing quality determines confidence in safe refactoring and releases"
        UseCase = "building fast feedback loops and meaningful regression safety nets"
        Tradeoff = "broader coverage vs build time and maintenance overhead"
        Pitfall = "brittle tests that validate implementation details instead of behavior"
        Prereq = "xUnit basics, mocking concepts, and API behavior expectations."
    }
    "Configuration" = @{
        Focus = "environment-aware application configuration strategy"
        Why = "configuration errors cause major runtime failures"
        UseCase = "safely managing settings across local, CI, and production"
        Tradeoff = "centralized config controls vs deployment flexibility"
        Pitfall = "missing validation and secret handling discipline"
        Prereq = "ASP.NET configuration providers and environment layering basics."
    }
}

$defaultContext = @{
    Focus = "applied software engineering decision-making"
    Why = "it affects maintainability, reliability, and delivery outcomes"
    UseCase = "making consistent technical decisions under real constraints"
    Tradeoff = "implementation speed vs long-term system quality"
    Pitfall = "choosing patterns without validating production impact"
    Prereq = "Core .NET and ASP.NET fundamentals."
}

function Get-ModuleName([string]$relativePath) {
    if ($relativePath -match '^docs/([^/]+)/') { return $matches[1] }
    if ($relativePath -match '^docs/([^/]+)\.md$') { return $matches[1] }
    return "Runtime-Section-Map"
}

function Get-Title([string]$content, [string]$fallback) {
    $match = [regex]::Match($content, '(?m)^#\s+(.+)$')
    if ($match.Success) { return $match.Groups[1].Value.Trim() }
    return $fallback
}

function Get-RelatedExamples([string]$relativePath, [string]$moduleName) {
    $folderReadme = "docs/$moduleName/README.md"
    if (Test-Path (Join-Path $repoRoot $folderReadme)) { return $folderReadme }

    if ($relativePath -match '^docs/([^/]+)\.md$') {
        $candidate = "docs/$($matches[1])/README.md"
        if (Test-Path (Join-Path $repoRoot $candidate)) { return $candidate }
    }

    return "docs/README.md"
}

$changedFiles = Get-Content $changedListPath | Where-Object { $_ -and $_ -like "docs/*.md" }
$selectedFiles = @()
foreach ($file in $changedFiles) {
    $module = Get-ModuleName $file
    if ($Modules -contains $module) {
        $selectedFiles += $file
    }
}

$updatedCount = 0

foreach ($relativePath in $selectedFiles) {
    $fullPath = Join-Path $repoRoot $relativePath
    if (-not (Test-Path $fullPath)) { continue }

    $content = Get-Content -Path $fullPath -Raw
    $original = $content

    $moduleName = Get-ModuleName $relativePath
    $ctx = $contextByModule[$moduleName]
    if (-not $ctx) { $ctx = $defaultContext }

    $titleFallback = [System.IO.Path]::GetFileNameWithoutExtension($relativePath)
    $title = Get-Title $content $titleFallback
    $relatedExamples = Get-RelatedExamples $relativePath $moduleName

    $newPrereq = "- Prerequisites: $($ctx.Prereq)"
    $newRelated = "- Related examples: $relatedExamples"

    $content = [regex]::Replace(
        $content,
        '(?m)^- Prerequisites:\s+See module README for sequencing guidance\.\s*$',
        [System.Text.RegularExpressions.MatchEvaluator]{ param($m) $newPrereq }
    )
    $content = [regex]::Replace(
        $content,
        '(?m)^- Related examples:\s+README\.md\s*$',
        [System.Text.RegularExpressions.MatchEvaluator]{ param($m) $newRelated }
    )

    $answerBlock = @"
## Interview Answer Block
30-second answer:
- $title is about $($ctx.Focus). It matters because $($ctx.Why).
- Use it when $($ctx.UseCase).

2-minute answer:
- Start with the problem $title solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: $($ctx.Tradeoff).
- Close with one failure mode and mitigation: $($ctx.Pitfall).
"@

    $badStrongBlock = @"
## Interview Bad vs Strong Answer
Bad answer:
- Defines $title but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose $title, what to compare it against, and how to validate it in tests/operations.
"@

    $timedDrillBlock = @"
## Interview Timed Drill
- 60 seconds: define $title and map it to one concrete implementation in this module.
- 3 minutes: compare $title with an alternative, then walk through one failure mode and mitigation.
"@

    $content = [regex]::Replace(
        $content,
        '(?s)## Interview Answer Block\r?\n.*?(?=\r?\n## Interview Bad vs Strong Answer)',
        $answerBlock
    )
    $content = [regex]::Replace(
        $content,
        '(?s)## Interview Bad vs Strong Answer\r?\n.*?(?=\r?\n## Interview Timed Drill)',
        $badStrongBlock
    )
    $content = [regex]::Replace(
        $content,
        '(?s)## Interview Timed Drill\r?\n.*?(?=(\r?\n## )|\z)',
        $timedDrillBlock
    )

    if ($content -ne $original) {
        Set-Content -Path $fullPath -Value $content -NoNewline
        $updatedCount++
    }
}

Write-Output "Modules processed: $($Modules -join ', ')"
Write-Output "Files updated: $updatedCount"
