# Behavioral Questions

> Subject: [Interview-Preparation](../README.md)

## Behavioral Questions

### STAR Method Framework

**S**ituation: Context and background
**T**ask: Challenge or responsibility
**A**ction: Steps you took
**R**esult: Outcome and learnings

---

### Common Behavioral Questions

**Q: Tell me about a time you faced a difficult bug**

**Example Answer**:

- **Situation**: Production issue - API response time increased from 200ms to 5s
- **Task**: Identify root cause and fix without downtime
- **Action**:
  1. Checked monitoring dashboards (APM)
  2. Found N+1 query problem in EF Core
  3. Added `.Include()` for eager loading
  4. Deployed fix with feature flag
- **Result**: Response time back to 200ms, learned importance of query analysis

---

**Q: Describe a time you had to learn something quickly**

**Example Answer**:

- **Situation**: Project required SignalR for real-time features, had 1 week
- **Task**: Learn SignalR and implement chat feature
- **Action**:
  1. Read official docs
  2. Built prototype
  3. Code review with senior dev
  4. Implemented production feature
- **Result**: Delivered on time, became team's SignalR expert

---

**Q: Tell me about a time you disagreed with a team member**

**Example Answer**:

- **Situation**: Teammate wanted to use Repository pattern for all entities
- **Task**: Discuss trade-offs and reach consensus
- **Action**:
  1. Presented research on when Repository adds value
  2. Showed EF Core DbSet already provides repository functionality
  3. Proposed using it only for complex domain logic
- **Result**: Team agreed, reduced unnecessary abstraction

---


