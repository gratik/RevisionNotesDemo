# System Design Questions

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


