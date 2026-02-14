// ============================================================================
// COMMAND PATTERN
// Reference: Revision Notes - Design Patterns (Behavioral) - Page 3
// ============================================================================
// PURPOSE: "Encapsulates a request as an object, allowing parameterization and queuing."
// EXAMPLE: Undo/redo functionality in editors.
// ============================================================================

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
