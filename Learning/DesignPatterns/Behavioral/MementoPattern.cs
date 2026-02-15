// ==============================================================================
// MEMENTO PATTERN - Snapshot and Restore Object State
// Reference: Revision Notes - Design Patterns
// ==============================================================================
//
// WHAT IS THE MEMENTO PATTERN?
// ----------------------------
// Captures and externalizes an object's internal state so it can be restored later,
// without violating encapsulation. Provides undo/redo functionality by storing
// snapshots of object state at different points in time.
//
// Think of it as: "Video game save points - save your progress (memento) at any time,
// die in game? Reload from last save point to restore exact state."
//
// Core Concepts:
//   • Originator: Object whose state needs to be saved/restored
//   • Memento: Immutable snapshot of originator's state
//   • Caretaker: Stores mementos, doesn't know memento contents
//   • Encapsulation: Memento opaque to caretaker (only originator accesses internals)
//   • Time Travel: Restore to any previous state
//
// WHY IT MATTERS
// --------------
// ✅ UNDO/REDO: Implement unlimited undo by storing state history
// ✅ ENCAPSULATION: External objects can't see internal state
// ✅ SNAPSHOTS: Checkpoint system for rollback
// ✅ SIMPLICITY: Cleaner than exposing all internal state via getters/setters
// ✅ DECOUPLING: Caretaker doesn't depend on originator implementation
//
// WHEN TO USE IT
// --------------
// ✅ Need undo/redo functionality
// ✅ Want to save/restore object state
// ✅ Direct access to state would violate encapsulation
// ✅ Implementing checkpoint/rollback system
// ✅ Snapshotting for backup or testing
//
// WHEN NOT TO USE IT
// ------------------
// ❌ State is small and cheap to expose (use property getters)
// ❌ State is huge (mementos consume too much memory)
// ❌ Deep object graphs (expensive to clone)
// ❌ Frequent state changes (too many mementos)
//
// REAL-WORLD EXAMPLE - Text Editor Undo/Redo
// ------------------------------------------
// Microsoft Word / VS Code undo system:
//   • User types: "Hello World"
//   • Each change creates a memento:
//     1. Memento 1: "H" (cursor: 1)
//     2. Memento 2: "He" (cursor: 2)
//     3. Memento 3: "Hel" (cursor: 3)
//     ...
//     11. Memento 11: "Hello World" (cursor: 11)
//   • User presses Ctrl+Z (undo) → Restore Memento 10: "Hello Worl"
//   • User presses Ctrl+Y (redo) → Restore Memento 11: "Hello World"
//
// WITHOUT MEMENTO:
//   ❌ class TextEditor {
//         public string Content { get; set; }
//         public int Cursor { get; set; }
//         // How to save state? Expose everything?
//         // Where to store history? Who manages it?
//     }
//   ❌ No encapsulation
//   ❌ Undo logic mixed with editor logic
//
// WITH MEMENTO:
//   ✅ class TextEditor {  // Originator
//         private string _content;
//         private int _cursor;
//         
//         public Memento SaveState() {
//             return new Memento(_content, _cursor); // Create snapshot
//         }
//         
//         public void RestoreState(Memento memento) {
//             _content = memento.Content;
//             _cursor = memento.Cursor;
//         }
//     }
//   
//   ✅ class Memento {  // Immutable snapshot
//         public string Content { get; }
//         public int Cursor { get; }
//         public Memento(string content, int cursor) {
//             Content = content;
//             Cursor = cursor;
//         }
//     }
//   
//   ✅ class History {  // Caretaker
//         private Stack<Memento> _undoStack = new();
//         private Stack<Memento> _redoStack = new();
//         
//         public void Save(Memento memento) {
//             _undoStack.Push(memento);
//             _redoStack.Clear(); // Clear redo on new action
//         }
//         
//         public Memento Undo() => _undoStack.Pop();
//         public Memento Redo() => _redoStack.Pop();
//     }
//   
//   ✅ Usage:
//     var editor = new TextEditor();
//     var history = new History();
//     
//     editor.Type("Hello");
//     history.Save(editor.SaveState()); // Save checkpoint
//     
//     editor.Type(" World");
//     history.Save(editor.SaveState());
//     
//     editor.RestoreState(history.Undo()); // Undo → "Hello"
//
// ANOTHER EXAMPLE - Database Transaction Rollback
// -----------------------------------------------
// SQL transaction with savepoints:
//   BEGIN TRANSACTION
//     UPDATE Accounts SET Balance = Balance - 100 WHERE Id = 1;
//     SAVE TRANSACTION Savepoint1;  -- Memento!
//     
//     UPDATE Accounts SET Balance = Balance + 100 WHERE Id = 2;
//     -- Oops, error!
//     ROLLBACK TRANSACTION Savepoint1;  -- Restore memento
//   COMMIT TRANSACTION
//
// Code equivalent:
//   var db = new Database();
//   var memento = db.CreateSavepoint();  // Save state
//   db.UpdateAccount(1, -100);
//   db.UpdateAccount(2, +100);  // Error!
//   db.RollbackToSavepoint(memento);    // Restore
//
// ANOTHER EXAMPLE - Game State Save/Load
// --------------------------------------
// RPG save system (Skyrim, Witcher):
//   • State includes:
//     - Player position (x, y, z)
//     - Health, mana, stamina
//     - Inventory (100+ items)
//     - Quest progress
//     - NPC states
//     - World changes
//   • Save game = create memento of entire game state
//   • Load game = restore from memento
//
// Code:
//   class Game {
//       public GameMemento SaveGame() {
//           return new GameMemento(
//               player.Position,
//               player.Health,
//               inventory.Clone(),
//               quests.Clone(),
//               worldState.Clone()
//           );
//       }
//       
//       public void LoadGame(GameMemento memento) {
//           player.Position = memento.Position;
//           player.Health = memento.Health;
//           inventory = memento.Inventory;
//           // ... restore everything
//       }
//   }
//
// ANOTHER EXAMPLE - Form Wizard with Back Button
// ----------------------------------------------
// Multi-step form (checkout wizard):
//   • Step 1: Shipping address
//   • Step 2: Payment method
//   • Step 3: Review order
//   • User clicks "Back" → Restore previous step's form state
//
// Code:
//   class CheckoutWizard {
//       private Stack<FormMemento> _history = new();
//       
//       public void GoToNextStep() {
//           _history.Push(SaveFormState());  // Save before moving
//           MoveToNextStep();
//       }
//       
//       public void GoBack() {
//           RestoreFormState(_history.Pop());  // Restore previous
//       }
//   }
//
// MEMENTO IMPLEMENTATION PATTERNS
// -------------------------------
// Pattern 1: Nested Memento Class (Encapsulation)
//   class Editor {
//       private string _content;
//       
//       public class Memento {  // Nested = can access private
//           private readonly string _content;
//           internal Memento(string content) => _content = content;
//           internal string GetContent() => _content;
//       }
//       
//       public Memento Save() => new Memento(_content);
//       public void Restore(Memento m) => _content = m.GetContent();
//   }
//
// Pattern 2: Memento Interface (Opaque to Caretaker)
//   interface IMemento { } // Empty interface
//   class ConcreteMemento : IMemento {
//       internal string State { get; }  // Only originator can access
//   }
//
// MEMORY CONSIDERATIONS
// ---------------------
// Problem: Mementos can consume lots of memory
//   • Large objects
//   • Frequent saves
//   • Long history
//
// Solutions:
// 1. **Incremental Mementos** (save only changes)
//    Instead of: Full state each time
//    Use: Delta/diff from previous state
//
// 2. **Limit History Size**
//    Keep only last N mementos (e.g., 50 undo levels)
//
// 3. **Compression**
//    Compress memento data before storing
//
// 4. **Lazy Loading**
//    Store mementos on disk, load when needed
//
// 5. **Command Pattern Hybrid**
//    Store commands instead of full state (replay to reconstruct)
//
// MEMENTO + COMMAND PATTERN
// -------------------------
// Combined for powerful undo/redo:
//   • Command: Stores action + parameters (lightweight)
//   • Memento: Stores full state (heavyweight)
//   • Use commands when possible, mementos when necessary
//
// Example:
//   class TextCommand : ICommand {
//       private Memento _beforeState;
//       public void Execute() {
//           _beforeState = editor.SaveState();  // Memento
//           editor.InsertText("abc");           // Command
//       }
//       public void Undo() {
//           editor.RestoreState(_beforeState);  // Use memento
//       }
//   }
//
// .NET FRAMEWORK EXAMPLES
// -----------------------
// Memento-like patterns in .NET:
//   • ICloneable: Create copy of object state
//   • Serialization: Serialize state to bytes/JSON
//   • DataSet.GetChanges(): Snapshot of changes
//   • Transaction: Database savepoints
//   • ViewState (ASP.NET WebForms): Page state snapshots
//
// SERIALIZATION AS MEMENTO
// ------------------------
// Modern approach using JSON:
//   class Editor {
//       public string SaveState() {
//           return JsonSerializer.Serialize(this); // Memento as JSON
//       }
//       
//       public void RestoreState(string json) {
//           var state = JsonSerializer.Deserialize<Editor>(json);
//           // Copy state from deserialized object
//       }
//   }
//
// BEST PRACTICES
// --------------
// ✅ Make mementos immutable (prevent tampering)
// ✅ Keep memento opaque to caretaker
// ✅ Use nested classes for tight encapsulation
// ✅ Consider memory usage (limit history, use incremental saves)
// ✅ Timestamp mementos for debugging
// ✅ Implement IEquatable for memento comparison
// ✅ Consider Command pattern for lightweight undo
//
// MEMENTO VS SIMILAR PATTERNS
// ---------------------------
// Memento vs Command:
//   • Memento: Stores state snapshots
//   • Command: Stores operations/actions
//   • Often used together for undo
//
// Memento vs Prototype:
//   • Memento: Opaque snapshot for later restoration
//   • Prototype: Full clone for immediate use
//
// ==========================================================================================================================================================

namespace RevisionNotesDemo.DesignPatterns.Behavioral;

// ========================================================================
// MEMENTO - Stores internal state
// ========================================================================

public class TextEditorMemento
{
    public string Content { get; }
    public int CursorPosition { get; }
    public DateTime Timestamp { get; }

    public TextEditorMemento(string content, int cursorPosition)
    {
        Content = content;
        CursorPosition = cursorPosition;
        Timestamp = DateTime.Now;
    }

    public override string ToString() =>
        $"[Memento {Timestamp:HH:mm:ss}] Content: \"{Content}\" | Cursor: {CursorPosition}";
}

// ========================================================================
// ORIGINATOR - Creates and restores from memento
// ========================================================================

public class TextEditor
{
    private string _content = string.Empty;
    private int _cursorPosition = 0;

    public void Type(string text)
    {
        _content += text;
        _cursorPosition = _content.Length;
        Console.WriteLine($"[EDITOR] ✍️  Typed: \"{text}\" -> Content: \"{_content}\"");
    }

    public void SetCursor(int position)
    {
        _cursorPosition = Math.Max(0, Math.Min(position, _content.Length));
        Console.WriteLine($"[EDITOR] 📍 Cursor moved to position {_cursorPosition}");
    }

    public void DeleteLast(int count)
    {
        if (_content.Length >= count)
        {
            _content = _content.Substring(0, _content.Length - count);
            _cursorPosition = _content.Length;
            Console.WriteLine($"[EDITOR] 🗑️  Deleted {count} characters -> Content: \"{_content}\"");
        }
    }

    public string GetContent() => _content;

    // Save state to memento
    public TextEditorMemento Save()
    {
        Console.WriteLine($"[EDITOR] 💾 Saving state...");
        return new TextEditorMemento(_content, _cursorPosition);
    }

    // Restore state from memento
    public void Restore(TextEditorMemento memento)
    {
        _content = memento.Content;
        _cursorPosition = memento.CursorPosition;
        Console.WriteLine($"[EDITOR] ↩️  Restored: \"{_content}\" (cursor: {_cursorPosition})");
    }

    public override string ToString() =>
        $"Content: \"{_content}\" | Cursor: {_cursorPosition}";
}

// ========================================================================
// CARETAKER - Manages mementos (history)
// ========================================================================

public class EditorHistory
{
    private readonly Stack<TextEditorMemento> _undoStack = new();
    private readonly Stack<TextEditorMemento> _redoStack = new();

    public void Save(TextEditorMemento memento)
    {
        _undoStack.Push(memento);
        _redoStack.Clear();  // Clear redo stack on new save
        Console.WriteLine($"[HISTORY] 📚 Saved state (undo stack: {_undoStack.Count})");
    }

    public TextEditorMemento? Undo()
    {
        if (_undoStack.Count == 0)
        {
            Console.WriteLine("[HISTORY] ⚠️  Nothing to undo");
            return null;
        }

        var memento = _undoStack.Pop();
        _redoStack.Push(memento);
        Console.WriteLine($"[HISTORY] ⬅️  Undo (undo: {_undoStack.Count}, redo: {_redoStack.Count})");

        return _undoStack.Count > 0 ? _undoStack.Peek() : null;
    }

    public TextEditorMemento? Redo()
    {
        if (_redoStack.Count == 0)
        {
            Console.WriteLine("[HISTORY] ⚠️  Nothing to redo");
            return null;
        }

        var memento = _redoStack.Pop();
        _undoStack.Push(memento);
        Console.WriteLine($"[HISTORY] ➡️  Redo (undo: {_undoStack.Count}, redo: {_redoStack.Count})");

        return memento;
    }

    public void ShowHistory()
    {
        Console.WriteLine("\n[HISTORY] 📜 Undo Stack:");
        if (_undoStack.Count == 0)
        {
            Console.WriteLine("  (empty)");
        }
        else
        {
            foreach (var memento in _undoStack)
                Console.WriteLine($"  {memento}");
        }
        Console.WriteLine();
    }
}

// ========================================================================
// EXAMPLE 2: GAME STATE
// ========================================================================

public class GameStateMemento
{
    public int Level { get; }
    public int Score { get; }
    public int Lives { get; }
    public string CheckpointName { get; }

    public GameStateMemento(int level, int score, int lives, string checkpointName)
    {
        Level = level;
        Score = score;
        Lives = lives;
        CheckpointName = checkpointName;
    }

    public override string ToString() =>
        $"[{CheckpointName}] Level {Level} | Score: {Score} | Lives: {Lives}";
}

public class Game
{
    public int Level { get; private set; } = 1;
    public int Score { get; private set; } = 0;
    public int Lives { get; private set; } = 3;

    public void Play(int pointsEarned, bool levelComplete = false)
    {
        Score += pointsEarned;
        if (levelComplete)
        {
            Level++;
            Console.WriteLine($"[GAME] 🎮 Level {Level - 1} complete! Advanced to Level {Level}");
        }
        else
        {
            Console.WriteLine($"[GAME] 🎮 Playing... Score: {Score}");
        }
    }

    public void LoseLife()
    {
        Lives--;
        Console.WriteLine($"[GAME] 💔 Lost a life! Lives remaining: {Lives}");
    }

    public GameStateMemento SaveCheckpoint(string checkpointName)
    {
        Console.WriteLine($"[GAME] 💾 Checkpoint saved: {checkpointName}");
        return new GameStateMemento(Level, Score, Lives, checkpointName);
    }

    public void LoadCheckpoint(GameStateMemento memento)
    {
        Level = memento.Level;
        Score = memento.Score;
        Lives = memento.Lives;
        Console.WriteLine($"[GAME] ↩️  Checkpoint loaded: {memento}");
    }

    public override string ToString() =>
        $"Level {Level} | Score: {Score} | Lives: {Lives}";
}

// ========================================================================
// DEMONSTRATION
// ========================================================================

public class MementoDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== MEMENTO PATTERN DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Design Patterns\n");

        // Example 1: Text Editor with Undo/Redo
        Console.WriteLine("=== EXAMPLE 1: Text Editor with Undo/Redo ===\n");

        var editor = new TextEditor();
        var history = new EditorHistory();

        // Initial save
        history.Save(editor.Save());

        // Make changes
        editor.Type("Hello");
        history.Save(editor.Save());

        editor.Type(" World");
        history.Save(editor.Save());

        editor.Type("!");
        history.Save(editor.Save());

        Console.WriteLine($"\nCurrent state: {editor}\n");

        // Undo operations
        Console.WriteLine("--- Undo Operations ---");
        var memento = history.Undo();
        if (memento != null) editor.Restore(memento);

        memento = history.Undo();
        if (memento != null) editor.Restore(memento);

        Console.WriteLine($"\nAfter 2 undos: {editor}\n");

        // Redo operations
        Console.WriteLine("--- Redo Operations ---");
        memento = history.Redo();
        if (memento != null) editor.Restore(memento);

        Console.WriteLine($"\nAfter 1 redo: {editor}\n");

        // New edit clears redo stack
        Console.WriteLine("--- New Edit (clears redo stack) ---");
        editor.Type("!!!");
        history.Save(editor.Save());

        memento = history.Redo();  // Should show "nothing to redo"

        // Show history
        history.ShowHistory();

        // Example 2: Game Checkpoints
        Console.WriteLine("\n=== EXAMPLE 2: Game Checkpoints ===\n");

        var game = new Game();
        var checkpoints = new List<GameStateMemento>();

        Console.WriteLine("--- Playing Level 1 ---");
        game.Play(100);
        game.Play(150);
        checkpoints.Add(game.SaveCheckpoint("Level 1 Complete"));
        game.Play(200, levelComplete: true);
        Console.WriteLine($"Current state: {game}\n");

        Console.WriteLine("--- Playing Level 2 ---");
        game.Play(300);
        checkpoints.Add(game.SaveCheckpoint("Midway Level 2"));
        game.Play(250);
        game.LoseLife();
        game.LoseLife();
        Console.WriteLine($"Current state: {game}\n");

        Console.WriteLine("--- Game Over! Loading Last Checkpoint ---");
        game.LoadCheckpoint(checkpoints[1]);  // Reload "Midway Level 2"
        Console.WriteLine($"Restored state: {game}\n");

        Console.WriteLine("--- Continue Playing ---");
        game.Play(400, levelComplete: true);
        Console.WriteLine($"Final state: {game}\n");

        Console.WriteLine("💡 Memento Pattern Benefits:");
        Console.WriteLine("   ✅ Undo/Redo functionality");
        Console.WriteLine("   ✅ Preserves encapsulation - doesn't expose internal state");
        Console.WriteLine("   ✅ Snapshot capability - save state at any point");
        Console.WriteLine("   ✅ Rollback on errors - restore previous valid state");
        Console.WriteLine("   ✅ History management - navigate through states");

        Console.WriteLine("\n💡 Real-World Examples:");
        Console.WriteLine("   • Text editors (undo/redo)");
        Console.WriteLine("   • Games (checkpoints, save/load)");
        Console.WriteLine("   • Database transactions (rollback)");
        Console.WriteLine("   • Version control systems");
        Console.WriteLine("   • Form wizards (back button)");
    }
}