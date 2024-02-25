namespace QuickNote.Core;

public struct TodoCollection<T>
    where T : IMarkdownReadable<T>
{

    private DateTime identifier;

    public DateTime Identifier
    {
        get => identifier.Date;
        set => identifier = value;
    }

    public IEnumerable<T> Todos { get; init; }

    public TodoCollection(DateTime identifier, IEnumerable<T> todos)
    {
        Identifier = identifier;
        Todos = todos;
    }
}
