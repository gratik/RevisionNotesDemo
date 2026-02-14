// ==============================================================================
// MEMENTO PATTERN
// Reference: Revision Notes - Design Patterns
// ==============================================================================
// PURPOSE: Capture and restore object state without violating encapsulation
// BENEFIT: Undo/redo functionality, snapshots, rollback capability
// USE WHEN: Need to save/restore state, implement undo, checkpoint/restore functionality
// ==============================================================================

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