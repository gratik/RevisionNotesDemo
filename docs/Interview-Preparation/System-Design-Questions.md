# System Design Questions

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Comfort with core module topics and deliberate timed practice.
- Related examples: docs/Interview-Preparation/README.md
> Subject: [Interview-Preparation](../README.md)

## System Design Questions

### Design a URL Shortener

**Requirements**:

- Shorten long URLs to short codes
- Redirect short → long URL
- Analytics (click count)
- Expiration

**High-Level Design**:

```
User → API Gateway → URL Service → Database (Redis + SQL)
                          ↓
                     Cache Layer (Redis)
```

**Key Components**:

1. **URL Generation Service**
   - Base62 encoding (a-z, A-Z, 0-9)
   - 7 characters = 62^7 = 3.5 trillion URLs
2. **Database Schema**:

   ```sql
   CREATE TABLE Urls (
       Id BIGINT PRIMARY KEY,
       ShortCode VARCHAR(10) UNIQUE,
       LongUrl VARCHAR(2048),
       CreatedAt DATETIME,
       ExpiresAt DATETIME,
       ClickCount INT
   );

   INDEX on ShortCode for fast lookup
   ```

3. **Redis Cache**:
   - Key: short code
   - Value: long URL
   - TTL: 24 hours

4. **API Endpoints**:
   ```
   POST /api/shorten → Create short URL
   GET /{shortCode} → Redirect to long URL
   GET /api/stats/{shortCode} → Get analytics
   ```

**Scaling**:

- Read-heavy → Cache with Redis
- Write-heavy → Database sharding by hash(shortCode)
- CDN for static content
- Load balancer for multiple API instances

---

### Design a Rate Limiter

**Requirements**:

- Limit API calls per user
- Different limits per tier (free/premium)
- Track requests across distributed servers

**Algorithms**:

1. **Token Bucket** (Recommended):
   - Bucket with fixed capacity
   - Tokens added at fixed rate
   - Request consumes token
   - If no tokens → reject

2. **Sliding Window Log**:
   - Track timestamp of each request
   - Count requests in last N seconds

**Implementation with Redis**:

```csharp
public class RateLimiter
{
    private readonly IDatabase _redis;

    public async Task<bool> IsAllowedAsync(string userId, int maxRequests, TimeSpan window)
    {
        string key = $"rate_limit:{userId}";
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long windowStart = now - (long)window.TotalSeconds;

        // Remove old entries
        await _redis.SortedSetRemoveRangeByScoreAsync(key, 0, windowStart);

        // Count requests in window
        long count = await _redis.SortedSetLengthAsync(key);

        if (count < maxRequests)
        {
            // Add current request
            await _redis.SortedSetAddAsync(key, now, now);
            await _redis.KeyExpireAsync(key, window);
            return true;
        }

        return false;  // Rate limit exceeded
    }
}
```

---


## Interview Answer Block
30-second answer:
- System Design Questions is about communication structure for technical interviews. It matters because clear articulation of tradeoffs improves interview signal quality.
- Use it when translating implementation knowledge into concise answers.

2-minute answer:
- Start with the problem System Design Questions solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: brevity vs sufficient technical depth.
- Close with one failure mode and mitigation: memorized answers that ignore problem context.
## Interview Bad vs Strong Answer
Bad answer:
- Defines System Design Questions but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose System Design Questions, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define System Design Questions and map it to one concrete implementation in this module.
- 3 minutes: compare System Design Questions with an alternative, then walk through one failure mode and mitigation.