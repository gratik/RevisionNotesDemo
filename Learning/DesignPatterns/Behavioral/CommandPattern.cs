// ==============================================================================
// COMMAND PATTERN - Turn Requests Into Objects for Undo/Redo/Queue
// Reference: Revision Notes - Design Patterns (Behavioral) - Page 3
// ==============================================================================
//
// WHAT IS THE COMMAND PATTERN?
// ----------------------------
// Encapsulates a request as an object, allowing you to parameterize clients with
// different requests, queue requests, log operations, and support undo/redo.
// Turns method calls into first-class objects that can be stored, passed, and manipulated.
//
// Think of it as: "TV remote control - buttons are commands. Each button (Command)
// encapsulates action (ON, OFF, Volume Up). Remote doesn't know how TV works,
// just executes commands. Can also undo last command."
//
// Core Concepts:
//   • Command Interface: Declares Execute() and optionally Undo()
//   • Concrete Command: Encapsulates action + receiver + parameters
//   • Receiver: Object that actually performs the work
//   • Invoker: Asks command to execute (e.g., button, menu item)
//   • Client: Creates commands and assigns to invoker
//
// WHY IT MATTERS
// --------------
// ✅ UNDO/REDO: Store command history, revert operations
// ✅ DECOUPLE SENDER FROM RECEIVER: Invoker doesn't know receiver details
// ✅ QUEUING: Commands can be queued, scheduled, or logged
// ✅ MACRO COMMANDS: Combine multiple commands (composite pattern)
// ✅ TRANSACTION SUPPORT: Execute batch of commands atomically
// ✅ LOGGING/AUDITING: Record all operations for replay or debugging
//
// WHEN TO USE IT
// --------------
// ✅ Need undo/redo functionality (text editors, graphics apps)
// ✅ Queue operations for later execution (job scheduler, task queue)
// ✅ Log operations (audit trail, crash recovery)
// ✅ Parameterize objects with operations (GUI callbacks)
// ✅ Support macro recording (combine multiple commands)
// ✅ Implement transactional behavior (all-or-nothing execution)
// ✅ Decouple object that invokes operation from object that performs it
//
// WHEN NOT TO USE IT
// ------------------
// ❌ Simple method calls without undo/queue/log needs
// ❌ Operations can't be parameterized or undone
// ❌ Overhead of creating command objects not justified
// ❌ Direct method calls are clearer and sufficient
//
// REAL-WORLD EXAMPLE - Text Editor (Microsoft Word)
// -------------------------------------------------
// Text editor with undo/redo:
//   • User actions:
//     1. Type "Hello" → InsertTextCommand("Hello")
//     2. Type " World" → InsertTextCommand(" World")
//     3. Bold selection → FormatCommand(FontWeight.Bold)
//     4. Delete word → DeleteTextCommand(length: 5)
//     5. Ctrl+Z (Undo) → commandHistory.Pop().Undo()
//     6. Ctrl+Y (Redo) → redoStack.Pop().Execute()
//
// Command History Stack:
//   [InsertTextCommand("Hello")]
//   [InsertTextCommand(" World")]
//   [FormatCommand(Bold)]
//   [DeleteTextCommand(5)]
//
// Undo operation:
//   • Pop DeleteTextCommand from history
//   • Call Undo() → Re-insert deleted text
//   • Push to redo stack
//
// WITHOUT COMMAND:
//   ❌ editor.InsertText("Hello"); // How to undo?
//   ❌ No history of operations
//   ❌ Can't replay operations
//   ❌ Can't implement undo/redo
//
// WITH COMMAND:
//   ✅ var cmd = new InsertTextCommand(editor, "Hello");
//   ✅ cmd.Execute(); // Perform action
//   ✅ commandHistory.Push(cmd); // Store for undo
//   ✅ Undo: cmd.Undo(); // Revert action
//   ✅ Save document: Replay all commands from empty state
//
// ANOTHER EXAMPLE - Smart Home Automation
// ---------------------------------------
// Home Assistant / Alexa routines:
//   • "Good Morning" routine:
//     1. TurnOnLightsCommand(brightness: 50%)
//     2. SetThermostatCommand(temp: 72°F)
//     3. OpenBlindsCommand(position: 100%)
//     4. StartCoffeeMakerCommand()
//     5. PlayMusicCommand(playlist: "Morning Jazz")
//   
//   • "Good Night" routine (undo morning):
//     1. TurnOffLightsCommand()
//     2. SetThermostatCommand(temp: 68°F)
//     3. CloseBlindsCommand()
//     4. LockDoorsCommand()
//     5. ActivateAlarmCommand()
//
// Macro command:
//   class MacroCommand : ICommand {
//       private List<ICommand> _commands;
//       public void Execute() => _commands.ForEach(c => c.Execute());
//       public void Undo() => _commands.Reverse().ForEach(c => c.Undo());
//   }
//   
//   var goodMorning = new MacroCommand(
//       new TurnOnLightsCommand(50),
//       new SetThermostatCommand(72),
//       new OpenBlindsCommand(100)
//   );
//   
//   alexaButton.SetCommand(goodMorning); // One button executes all
//
// ANOTHER EXAMPLE - Restaurant Order System
// -----------------------------------------
// Restaurant kitchen order queue:
//   • Waiter takes order → creates OrderCommand
//   • Commands queued in kitchen
//   • Chef processes commands FIFO
//   • Each command encapsulates:
//     - Table number
//     - Menu items
//     - Special instructions
//     - Timestamp
//
// Code:
//   interface IOrderCommand { void Execute(); void Cancel(); }
//   
//   class OrderCommand : IOrderCommand {
//       private Chef _chef; // Receiver
//       private Order _order;
//       public void Execute() => _chef.PrepareOrder(_order);
//       public void Cancel() => _chef.CancelOrder(_order);
//   }
//   
//   // Waiter (Invoker)
//   class Waiter {
//       private Queue<IOrderCommand> _orderQueue;
//       public void TakeOrder(IOrderCommand cmd) {
//           _orderQueue.Enqueue(cmd); // Queue for later
//           Console.WriteLine($"Order queued: {cmd}");
//       }
//   }
//   
//   // Chef processes queue
//   while (_orderQueue.Any()) {
//       var cmd = _orderQueue.Dequeue();
//       cmd.Execute(); // Cook meal
//   }
//
// Benefits:
//   • Orders can be queued during rush hour
//   • Can cancel order: cmd.Cancel()
//   • Can log all orders for analytics
//   • Can implement "redo last order"
//
// COMMAND PATTERN STRUCTURE
// -------------------------
// interface ICommand {
//     void Execute();
//     void Undo();
// }
//
// class ConcreteCommand : ICommand {
//     private Receiver _receiver;
//     private object _state; // Parameters
//     
//     public void Execute() {
//         _state = _receiver.GetState(); // Save for undo
//         _receiver.Action();
//     }
//     
//     public void Undo() {
//         _receiver.RestoreState(_state); // Revert
//     }
// }
//
// class Invoker {
//     private ICommand _command;
//     public void SetCommand(ICommand cmd) => _command = cmd;
//     public void ExecuteCommand() => _command.Execute();
// }
//
// TRANSACTION SUPPORT
// -------------------
// Execute multiple commands atomically:
//   class TransactionCommand : ICommand {
//       private List<ICommand> _commands;
//       
//       public void Execute() {
//           try {
//               foreach (var cmd in _commands) {
//                   cmd.Execute();
//               }
//           } catch {
//               // Rollback all executed commands
//               foreach (var cmd in _commands.Executed) {
//                   cmd.Undo();
//               }
//               throw;
//           }
//       }
//   }
//
// Use case: Bank transfer
//   var transfer = new TransactionCommand(
//       new DebitAccountCommand(fromAccount, 100),
//       new CreditAccountCommand(toAccount, 100)
//   );
//   transfer.Execute(); // Either both succeed or both rollback
//
// .NET FRAMEWORK EXAMPLES
// -----------------------
// Command pattern in .NET:
//   • WPF/MAUI: ICommand (RelayCommand, DelegateCommand)
//   • LINQ: Deferred execution (commands queued, executed later)
//   • Task Parallel Library: Task.Run(() => ...) (action as command)
//   • MediatR: IRequest<T> / IRequestHandler<T> (CQRS commands)
//   • Thread pool: ThreadPool.QueueUserWorkItem (queue commands)
//
// COMMAND VS STRATEGY VS STATE
// ----------------------------
// Command vs Strategy:
//   • Command: What to do (encapsulate request) + undo/queue/log
//   • Strategy: How to do it (algorithm selection)
//
// Command vs Memento:
//   • Command: Stores operations to undo them
//   • Memento: Stores object state snapshots to restore them
//   • Often used together: Command uses Memento to store state for undo
//
// BEST PRACTICES
// --------------
// ✅ Store state needed for undo in command
// ✅ Keep commands small and focused (Single Responsibility)
// ✅ Use MacroCommand for composite commands
// ✅ Implement IDisposable if command holds resources
// ✅ Consider async commands for long-running operations
// ✅ Log commands for debugging and audit trails
// ✅ Use command history stack with max size (prevent memory leaks)
//
// ==============================================================================

namespace RevisionNotesDemo.DesignPatterns.Behavioral;

// Command interface
public interface ICommand
{
    void Execute();
    void Undo();
}

// Receiver
public class CommandTextEditor
{
    private string _content = string.Empty;

    public void InsertText(string text)
    {
        _content += text;
        Console.WriteLine($"[COMMAND] Inserted: '{text}' | Content: '{_content}'");
    }

    public void DeleteText(int length)
    {
        if (length <= _content.Length)
        {
            _content = _content.Substring(0, _content.Length - length);
            Console.WriteLine($"[COMMAND] Deleted {length} chars | Content: '{_content}'");
        }
    }

    public string GetContent() => _content;
}

// Concrete commands
public class InsertTextCommand : ICommand
{
    private readonly CommandTextEditor _editor;
    private readonly string _text;

    public InsertTextCommand(CommandTextEditor editor, string text)
    {
        _editor = editor;
        _text = text;
    }

    public void Execute()
    {
        _editor.InsertText(_text);
    }

    public void Undo()
    {
        _editor.DeleteText(_text.Length);
    }
}

// Invoker
public class EditorInvoker
{
    private readonly Stack<ICommand> _commandHistory = new();
    private readonly Stack<ICommand> _redoStack = new();

    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        _commandHistory.Push(command);
        _redoStack.Clear(); // Clear redo stack on new command
    }

    public void Undo()
    {
        if (_commandHistory.Count > 0)
        {
            var command = _commandHistory.Pop();
            command.Undo();
            _redoStack.Push(command);
            Console.WriteLine("[COMMAND] Undo performed");
        }
        else
        {
            Console.WriteLine("[COMMAND] Nothing to undo");
        }
    }

    public void Redo()
    {
        if (_redoStack.Count > 0)
        {
            var command = _redoStack.Pop();
            command.Execute();
            _commandHistory.Push(command);
            Console.WriteLine("[COMMAND] Redo performed");
        }
        else
        {
            Console.WriteLine("[COMMAND] Nothing to redo");
        }
    }
}

// Usage demonstration
public class CommandDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== COMMAND PATTERN DEMO ===\n");

        var editor = new CommandTextEditor();
        var invoker = new EditorInvoker();

        // Execute commands
        invoker.ExecuteCommand(new InsertTextCommand(editor, "Hello"));
        invoker.ExecuteCommand(new InsertTextCommand(editor, " World"));
        invoker.ExecuteCommand(new InsertTextCommand(editor, "!"));

        Console.WriteLine($"\n[COMMAND] Final content: '{editor.GetContent()}'\n");

        // Undo
        invoker.Undo();
        invoker.Undo();

        Console.WriteLine($"\n[COMMAND] After undo: '{editor.GetContent()}'\n");

        // Redo
        invoker.Redo();

        Console.WriteLine($"\n[COMMAND] After redo: '{editor.GetContent()}'");
        Console.WriteLine("\n💡 Benefit: Supports undo/redo operations");
        Console.WriteLine("💡 Benefit: Decouples sender from receiver");
    }
}
