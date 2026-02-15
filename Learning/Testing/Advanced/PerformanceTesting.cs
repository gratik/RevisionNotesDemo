// ==============================================================================
// PERFORMANCE TESTING - Load Testing, Stress Testing, and Benchmarking
// ==============================================================================
// WHAT IS THIS?
// -------------
// Performance testing verifies that an application meets speed, scalability,
// and stability requirements under various load conditions. Tools like k6
// and JMeter simulate realistic user traffic to identify bottlenecks.
//
// WHY IT MATTERS
// --------------
// ✅ CAPACITY PLANNING: Know system limits before production
// ✅ BOTTLENECK DETECTION: Identify slow endpoints/databases
// ✅ SLA VERIFICATION: Confirm you meet response time promises
// ✅ REGRESSION DETECTION: Catch performance degradation early
// ✅ COST SAVINGS: Right-size infrastructure based on real load data
// ✅ USER EXPERIENCE: Ensure app stays responsive under load
//
// WHEN TO USE
// -----------
// ✅ Before major releases to production
// ✅ When scaling to new traffic levels
// ✅ After infrastructure changes
// ✅ When adding new features that impact performance
// ✅ Regular baseline testing (nightly/weekly)
//
// WHEN NOT TO USE
// ---------------
// ❌ On code that doesn't handle real traffic (pure logic)
// ❌ If you have no SLA requirements
// ❌ In early development before MVP
// ❌ Without proper test environment (not production!)
//
// REAL-WORLD EXAMPLE
// ------------------
// E-commerce platform launch:
// - Black Friday expected: 100x normal traffic
// - Run k6 load test: Simulate 10,000 concurrent users
// - Find MySQL connection pool too small (bottleneck)
// - Before launch: Increase pool and re-test
// - Launch day: Handle the load confidently
// ==============================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RevisionNotesDemo.Testing.Advanced;

public class PerformanceTesting
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║           PERFORMANCE TESTING - LOAD & STRESS TESTING      ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");
        
        LoadTestingTypes();
        K6Overview();
        JMeterOverview();
        DotNetBenchmarking();
        BestPractices();
    }

    private static void LoadTestingTypes()
    {
        Console.WriteLine("📊 PERFORMANCE TEST TYPES:\n");
        
        Console.WriteLine("1. LOAD TEST");
        Console.WriteLine("   - Simulate expected normal traffic");
        Console.WriteLine("   - Example: 1,000 users over 10 minutes");
        Console.WriteLine("   - Goal: Verify baseline performance\n");
        
        Console.WriteLine("2. STRESS TEST");
        Console.WriteLine("   - Exceed expected capacity");
        Console.WriteLine("   - Example: 10,000 users until system breaks");
        Console.WriteLine("   - Goal: Find breaking point and failure modes\n");
        
        Console.WriteLine("3. SPIKE TEST");
        Console.WriteLine("   - Sudden traffic increase");
        Console.WriteLine("   - Example: 100 to 5,000 users instantly");
        Console.WriteLine("   - Goal: Verify auto-scaling and recovery\n");
        
        Console.WriteLine("4. SOAK TEST");
        Console.WriteLine("   - Normal load over extended time");
        Console.WriteLine("   - Example: 500 users for 24 hours");
        Console.WriteLine("   - Goal: Detect memory leaks and degradation\n");
    }

    private static void K6Overview()
    {
        Console.WriteLine("⚙️  K6 (Modern Load Testing Tool):\n");
        
        Console.WriteLine("Installation: npm install -g k6\n");
        
        Console.WriteLine("JavaScript-based test scripts:");
        Console.WriteLine(@"
import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
  stages: [
    { duration: '30s', target: 20 },  // Ramp-up to 20 VUs
    { duration: '1m30s', target: 10 }, // Stay at 10 VUs
    { duration: '20s', target: 0 }     // Ramp-down
  ]
};

export default function () {
  let res = http.get('https://api.example.com/orders');
  
  check(res, {
    'status is 200': (r) => r.status === 200,
    'response time < 500ms': (r) => r.timings.duration < 500,
  });
  
  sleep(1);  // 1 second delay between requests
}
");

        Console.WriteLine("\nRun: k6 run load-test.js");
        Console.WriteLine("Output: Performance metrics, pass/fail criteria\n");
    }

    private static void JMeterOverview()
    {
        Console.WriteLine("⚙️  JMETER (Enterprise Load Testing Tool):\n");
        
        Console.WriteLine("Features:");
        Console.WriteLine("   • GUI test builder");
        Console.WriteLine("   • Advanced assertions and post-processors");
        Console.WriteLine("   • Distributed testing (master/slaves)");
        Console.WriteLine("   • Rich reporting and graphs");
        Console.WriteLine("   • Supports: HTTP, JDBC, SOAP, JMS, SMTP, etc.\n");
        
        Console.WriteLine("Test Plan Structure:");
        Console.WriteLine(@"
Test Plan
├─ Thread Group (500 users, 10 minute duration)
│  ├─ HTTP Sampler (GET /api/orders)
│  ├─ Response Assertion (status 200)
│  └─ Constant Timer (1000ms delay)
├─ Listener (View Results Tree)
└─ Listener (Aggregate Report)
");
    }

    private static void DotNetBenchmarking()
    {
        Console.WriteLine("⚙️  .NET BENCHMARKING (BenchmarkDotNet):\n");
        
        Console.WriteLine("Installation: dotnet add package BenchmarkDotNet\n");
        
        Console.WriteLine("Example - Compare algorithms:");
        Console.WriteLine(@"
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, targetCount: 5)]
public class AlgorithmBenchmarks
{
    private int[] numbers = Enumerable.Range(1, 1000).ToArray();
    
    [Benchmark]
    public int IntLinq()
    {
        return numbers.Where(x => x % 2 == 0).Sum();
    }
    
    [Benchmark]
    public int IntFor()
    {
        int sum = 0;
        foreach (var n in numbers)
            if (n % 2 == 0) sum += n;
        return sum;
    }
}

// Run: dotnet run -c Release
// Output shows: ops/sec, mean time, allocated memory
");
    }

    private static void BestPractices()
    {
        Console.WriteLine("\n✅ PERFORMANCE TESTING BEST PRACTICES:\n");
        
        Console.WriteLine("PREPARATION:");
        Console.WriteLine("   • Use production-like test environment");
        Console.WriteLine("   • Establish baseline metrics first");
        Console.WriteLine("   • Define success criteria (SLAs)");
        Console.WriteLine("   • Request types should match production patterns\n");
        
        Console.WriteLine("EXECUTION:");
        Console.WriteLine("   • Ramp up gradually (avoid thundering herd)");
        Console.WriteLine("   • Run tests multiple times for consistency");
        Console.WriteLine("   • Monitor infrastructure (CPU, memory, network)");
        Console.WriteLine("   • Test during off-peak hours\n");
        
        Console.WriteLine("ANALYSIS:");
        Console.WriteLine("   • Look for 95th and 99th percentiles, not just average");
        Console.WriteLine("   • Identify bottlenecks (CPU, memory, I/O)");
        Console.WriteLine("   • Check if failures are random or at specific load");
        Console.WriteLine("   • Compare against baseline and SLAs\n");
    }
}
