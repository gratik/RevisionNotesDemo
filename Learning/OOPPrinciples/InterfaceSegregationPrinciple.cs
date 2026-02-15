// ============================================================================
// INTERFACE SEGREGATION PRINCIPLE (ISP)
// Reference: Revision Notes - OOP (Object Oriented Principals) - Page 2
// ============================================================================
//
// WHAT IS ISP?
// ------------
// No client should be forced to depend on methods it does not use. Prefer
// small, focused interfaces over large, general-purpose ones.
//
// WHY IT MATTERS
// --------------
// - Reduces unnecessary coupling
// - Makes implementations clearer and smaller
// - Improves testability and reusability
// - Prevents empty or exception-throwing methods
//
// WHEN TO USE
// -----------
// - YES: When an interface grows too large
// - YES: When different clients need different subsets of behavior
// - YES: When implementations leave methods unused
//
// WHEN NOT TO USE
// ---------------
// - NO: When the interface represents a single cohesive capability
// - NO: When splitting the interface adds unnecessary complexity
//
// REAL-WORLD EXAMPLE
// ------------------
// Devices in a warehouse:
// - IScannable for barcode scanners
// - IPrintable for label printers
// - IWeighable for scales
// A barcode scanner should not be forced to implement PrintLabel().
// ============================================================================

namespace RevisionNotesDemo.OOPPrinciples;

// ❌ BAD EXAMPLE - Violates ISP
// Fat interface that forces classes to implement methods they don't need
public interface IWorkerBad
{
    void Work();
    void Eat();
    void Sleep();
    void GetPaid();
    void AttendMeeting();
}

// Human worker - implements all methods
public class HumanWorkerBad : IWorkerBad
{
    public void Work() => Console.WriteLine("Human working");
    public void Eat() => Console.WriteLine("Human eating");
    public void Sleep() => Console.WriteLine("Human sleeping");
    public void GetPaid() => Console.WriteLine("Human getting paid");
    public void AttendMeeting() => Console.WriteLine("Human attending meeting");
}

// Robot worker - forced to implement methods it doesn't need!
public class RobotWorkerBad : IWorkerBad
{
    public void Work() => Console.WriteLine("Robot working");

    // Robots don't eat, sleep, or attend meetings!
    public void Eat() => throw new NotImplementedException("Robots don't eat");
    public void Sleep() => throw new NotImplementedException("Robots don't sleep");
    public void GetPaid() => Console.WriteLine("Robot getting maintenance");
    public void AttendMeeting() => throw new NotImplementedException("Robots don't attend meetings");
}

// ✅ GOOD EXAMPLE - Follows ISP
// Segregated interfaces - clients only depend on what they need

public interface IWorkable
{
    void Work();
}

public interface IFeedable
{
    void Eat();
}

public interface ISleepable
{
    void Sleep();
}

public interface IPayable
{
    void GetPaid();
    decimal GetSalary();
}

public interface IMeetingAttendee
{
    void AttendMeeting(string meetingTopic);
}

// Human worker implements all applicable interfaces
public class HumanWorker : IWorkable, IFeedable, ISleepable, IPayable, IMeetingAttendee
{
    public string Name { get; }
    private decimal _salary;

    public HumanWorker(string name, decimal salary)
    {
        Name = name;
        _salary = salary;
    }

    public void Work()
    {
        Console.WriteLine($"[ISP] {Name} is working");
    }

    public void Eat()
    {
        Console.WriteLine($"[ISP] {Name} is eating lunch");
    }

    public void Sleep()
    {
        Console.WriteLine($"[ISP] {Name} is sleeping");
    }

    public void GetPaid()
    {
        Console.WriteLine($"[ISP] {Name} received payment: ${_salary}");
    }

    public decimal GetSalary() => _salary;

    public void AttendMeeting(string meetingTopic)
    {
        Console.WriteLine($"[ISP] {Name} is attending meeting about: {meetingTopic}");
    }
}

// Robot worker only implements what it needs
public class RobotWorker : IWorkable, IPayable
{
    public string Model { get; }
    private decimal _maintenanceCost;

    public RobotWorker(string model, decimal maintenanceCost)
    {
        Model = model;
        _maintenanceCost = maintenanceCost;
    }

    public void Work()
    {
        Console.WriteLine($"[ISP] Robot {Model} is working 24/7");
    }

    public void GetPaid()
    {
        Console.WriteLine($"[ISP] Robot {Model} maintenance budget allocated: ${_maintenanceCost}");
    }

    public decimal GetSalary() => _maintenanceCost;
}

// Contractor - works and gets paid, but doesn't need other benefits
public class Contractor : IWorkable, IPayable
{
    public string Name { get; }
    private decimal _hourlyRate;

    public Contractor(string name, decimal hourlyRate)
    {
        Name = name;
        _hourlyRate = hourlyRate;
    }

    public void Work()
    {
        Console.WriteLine($"[ISP] Contractor {Name} is working");
    }

    public void GetPaid()
    {
        Console.WriteLine($"[ISP] Contractor {Name} billed at ${_hourlyRate}/hour");
    }

    public decimal GetSalary() => _hourlyRate * 160; // Monthly estimate
}

// Manager service that works with segregated interfaces
public class WorkplaceManager
{
    public void ManageWorkers(IEnumerable<IWorkable> workers)
    {
        Console.WriteLine("\nManaging workers:");
        foreach (var worker in workers)
        {
            worker.Work();
        }
    }

    public void ProcessPayroll(IEnumerable<IPayable> payables)
    {
        Console.WriteLine("\nProcessing payroll:");
        foreach (var payable in payables)
        {
            payable.GetPaid();
        }
    }

    public void ScheduleMeeting(IEnumerable<IMeetingAttendee> attendees, string topic)
    {
        Console.WriteLine($"\nScheduling meeting: {topic}");
        foreach (var attendee in attendees)
        {
            attendee.AttendMeeting(topic);
        }
    }

    public void ScheduleBreaks(IEnumerable<IFeedable> feedableWorkers)
    {
        Console.WriteLine("\nScheduling lunch breaks:");
        foreach (var worker in feedableWorkers)
        {
            worker.Eat();
        }
    }
}

// Usage demonstration
public class ISPDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== INTERFACE SEGREGATION PRINCIPLE DEMO ===\n");

        var human = new HumanWorker("Alice", 5000m);
        var robot = new RobotWorker("R2D2", 500m);
        var contractor = new Contractor("Bob", 50m);

        var manager = new WorkplaceManager();

        // All can work
        var allWorkers = new List<IWorkable> { human, robot, contractor };
        manager.ManageWorkers(allWorkers);

        // All need payment
        var allPayables = new List<IPayable> { human, robot, contractor };
        manager.ProcessPayroll(allPayables);

        // Only humans can attend meetings (ISP benefit!)
        var meetingAttendees = new List<IMeetingAttendee> { human };
        manager.ScheduleMeeting(meetingAttendees, "Q1 Planning");

        // Only humans need lunch breaks (ISP benefit!)
        var feedableWorkers = new List<IFeedable> { human };
        manager.ScheduleBreaks(feedableWorkers);

        Console.WriteLine("\nBenefit: Each type implements only the interfaces it needs!");
        Console.WriteLine("No forced implementation of unused methods!");
    }
}
