using QuickNote.Core.Storage;

namespace QuickNote.Core;

/// <summary>
/// Inteface which represents classes that can notify the user on other channels 
/// </summary>
public interface ITodoNotifyable
{ 
    public bool State { get; set; }

    public void Notify(string message);
}

public enum TodoEventType
{
    CleanupReached = 0,
    EndDateReached = 1,
    TodoFinished = 2,
}

public class TodoEventArgs : EventArgs
{
    public TodoEventType Type { get; }

    public ITodoNotifyable? Notifier {  get; }

    public string Message { get; } = null!;

    public TodoEventArgs(TodoEventType type, string message) 
        : this(type, null, message)
    {

    }

    public TodoEventArgs(TodoEventType type, ITodoNotifyable? notifier, string message)
    {
        Type = type;
        Notifier = notifier;
        Message = message;
    }
}

public delegate void TodoEventHandler(object sender, TodoEventArgs e);

public class TodoApplication
{
    private StorageContext StoreContext { get; }

    private event TodoEventHandler eventHandler;

    public TodoApplication(string basepath)
    {
        MdWriterBuilder mwb = new MdWriterBuilder()
            .WithCheck()
            .WithName();

        MdReaderBuilder mrb = new MdReaderBuilder()
            .WithCheck()
            .WithName();

        StoreContext = new StorageContext(basepath, mrb, mwb);
    }

    protected virtual void OnTodoEvent(TodoEventArgs e)
    {
        eventHandler.Invoke(this, e);   
    }
    
    public async Task InitAsync()
    {
        DateTime yesterday = DateTime.Now.AddDays(-1);

        IEnumerable<Todo> todosYest = StoreContext
            .GetSingleAsync<Todo>(yesterday)
            .ToBlockingEnumerable()
            .Where(x => !x.IsFinished);

        List<Todo> current = new(todosYest);
        current.AddRange(StoreContext.GetSingleAsync<Todo>(DateTime.Now).ToBlockingEnumerable());

        await StoreContext.UpdateOrCreateAsync(DateTime.Now, current);

        await foreach (TodoCollection<Todo> todo in StoreContext.GetAllAsync<Todo>())
        {
            if (todo.Identifier.Date <= DateTime.Now.AddDays(-7))
                StoreContext.TryDelete(todo.Identifier);
        }
    }

    public void Append(Todo todo)
    {

    }


    public void RegisterEventHandler(TodoEventHandler eventHandler)
    {
        this.eventHandler += eventHandler;
    }

    public void RemoveEventHandler(TodoEventHandler eventHandler)
    {
        this.eventHandler -= eventHandler;
    }
}
