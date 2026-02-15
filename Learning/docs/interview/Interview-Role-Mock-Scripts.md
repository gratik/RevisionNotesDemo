# Interview Role Mock Scripts

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Interview Role Tracks
- Related examples: Learning/Architecture/IntegratedDomainSlicesCaseStudy.cs, Learning/DevOps/GitHubActionsWorkflows.cs

Use these scripts for role-focused mock interviews with explicit scoring signals.

## Script A: Backend .NET Engineer (30 minutes)

### Flow

1. Warm-up (5 min): Explain API contract design approach.
2. Core technical (15 min): Data access + async + validation strategy.
3. Reliability/security (7 min): Failure handling and auth boundaries.
4. Wrap-up (3 min): Tradeoff recap and one improvement plan.

### Sample questions

- How do you design a retry-safe order endpoint?
- When do you choose EF Core vs Dapper?
- What would you log to debug intermittent 500 errors?
- How do you enforce authorization across API and background handlers?

### Strong signals

- Mentions idempotency and problem details.
- Uses measurable language (p95/error-rate) when discussing reliability.
- Explains at least one tradeoff between simplicity and flexibility.

## Script B: Senior .NET Engineer (45 minutes)

### Flow

1. Architecture prompt (10 min): Service boundary and ownership decisions.
2. Deep dive (20 min): Performance/resilience failure mode walkthrough.
3. Leadership signal (10 min): Standards, reviews, and quality gates.
4. Wrap-up (5 min): What changed after incident learnings.

### Sample questions

- Describe a production failure and how you changed system design after it.
- How do you prevent retry storms in dependent-service outages?
- What CI gates do you require before merge and why?
- How do you decide if a pattern adds value or unnecessary complexity?

### Strong signals

- Gives one concrete incident with metrics before/after.
- Connects technical decisions to team-level standards.
- Shows prevention loop (monitoring, guardrails, runbooks, reviews).

## Script C: Platform / DevOps Engineer (35 minutes)

### Flow

1. Pipeline design (12 min): CI/CD checks and rollback strategy.
2. Operations depth (15 min): Monitoring, alerting, incident triage.
3. Governance (8 min): Suppression policy and quality ownership.

### Sample questions

- What would your minimum CI gate set include for this repository?
- How do you design deployment rollback criteria?
- Which observability signals are non-negotiable for production services?
- How do you balance velocity with strict quality controls?

### Strong signals

- Names concrete gates and failure behavior.
- Includes rollback trigger thresholds.
- Explains incident response with trace/metric/log correlation.

## Script D: Solutions Architect (45 minutes)

### Flow

1. Whiteboard scenario (20 min): End-to-end distributed workflow.
2. Consistency/security (15 min): Recovery, idempotency, and tenant boundaries.
3. Decision defense (10 min): Why this architecture over alternatives.

### Sample questions

- Design an order workflow across payment, inventory, and notification services.
- How do you preserve consistency without distributed transactions?
- How would this design evolve with 10x traffic and global regions?
- What governance artifacts prevent architecture drift?

### Strong signals

- Clear boundaries, contracts, and ownership.
- Explicit failure/recovery path with outbox/inbox thinking.
- Pragmatic tradeoff explanations (latency, complexity, operability).

## Scoring rubric (10 points)

- Technical correctness: 0-3
- Tradeoff articulation: 0-2
- Measurable outcomes: 0-2
- Communication clarity: 0-2
- Role alignment: 0-1

## Interview Answer Block

- 30-second answer: Role mock scripts turn broad prep into focused rehearsal with role-relevant question flow and evaluation criteria.
- 2-minute deep dive: I run timed role-specific rounds, evaluate with rubric, then target weak signals in the next practice cycle.
- Common follow-up: How do you choose which role script to practice first?
- Strong response: Start with target role, then practice adjacent role scripts to strengthen cross-functional depth.
- Tradeoff callout: Overfitting to one script can reduce adaptability in live interviews.

## Interview Bad vs Strong Answer

- Bad answer: “I practiced general interview questions.”
- Strong answer: “I rehearsed role-specific mock flows with scoring, tracked weak areas, and improved measurable signal quality each round.”
- Why strong wins: It demonstrates deliberate, evidence-driven preparation.

## Interview Timed Drill

- Time box: 15 minutes.
- Prompt: Pick one role script and answer two technical questions plus one tradeoff question.
- Required outputs:
  - One clear architecture or implementation decision
  - One metric-backed outcome statement
  - One explicit tradeoff rationale
- Self-check score (0-3 each): role fit, depth, measurable clarity.
