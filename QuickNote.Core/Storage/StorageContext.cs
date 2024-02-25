namespace QuickNote.Core.Storage;

public struct TodoCollection<T> 
    where T : IMarkdownReadable<T> {

    private DateTime identifier;

    public DateTime Identifier
    {
        get => identifier.Date;
        set => identifier = value;
    }

    public IEnumerable<T> Todos { get; init; }

    public TodoCollection(DateTime identifier, IEnumerable<T> todos) {
        Identifier = identifier;
        Todos = todos;
    }
}

/// <summary>
/// It's a proxy around the whole persistence/storage mechanism 
/// </summary>
public class StorageContext {

    public string Basepath { get; init; }

    private MarkdownReader Reader { get; init; }

    private MarkdownWriter Writer { get; init; }

    private Store Store { get; init; }

    public StorageContext(
        string basepath, 
        MdReaderBuilder rb, 
        MdWriterBuilder wb) {

        Basepath = basepath;
        Reader = rb.Build();
        Writer = wb.Build();
        Store = Store.FromFileSystem(basepath);
    }

    public async IAsyncEnumerable<T> GetSingleAsync<T>(DateTime identifier) 
        where T : IMarkdownReadable<T> {

        TodoFile? file = Store.MarkdownFiles
            .FirstOrNull(x => x.Identifier == identifier.Date);

        if (file is null) {
            yield break;
        }

        foreach (string? line in await file.Value.ReadLines) {
            MarkdownReader reader = Reader.Copy();
            IEnumerable<MarkdownNode> nodes = reader.Read(line);
            yield return T.Read(nodes);
        }
    }

    public async IAsyncEnumerable<TodoCollection<T>> GetAllAsync<T>() 
        where T : IMarkdownReadable<T> {

        IEnumerable<TodoFile> files = Store.MarkdownFiles.AsEnumerable();

        foreach (TodoFile file in files) {
            MarkdownReader reader = Reader.Copy();
            List<T> todos = new List<T>();
            foreach (string? line in await file.ReadLines) {
                IEnumerable<MarkdownNode> nodes = reader.Read(line);
                T res = T.Read(nodes);
                todos.Add(res);
            }
            yield return new TodoCollection<T>(file.Identifier, todos);
        }
    }

    private IEnumerable<string> ConvertNodes<T>(IEnumerable<T> todos) 
        where T : IMarkdownWriteable<T> {
            foreach (T todo in todos) {
                MarkdownWriter writer = Writer.Copy();
                yield return writer.Write(todo.Write());
            }
    }

    public async Task UpdateOrCreateAsync<T>(DateTime datetime, IEnumerable<T> todos) 
        where T : IMarkdownWriteable<T> {

        TodoFile? file = Store.MarkdownFiles
            .FirstOrDefault(x => x.Identifier.Date == datetime.Date);

        if (file is null) {
            file = TodoFile.CreateFrom(Basepath, datetime);
        }

        IEnumerable<string> result = ConvertNodes<T>(todos);
        file.Value.Append(() => result);
    }
}

file static class IEnumerableExtensions
{
    public static T? FirstOrNull<T>(this IEnumerable<T> entries, Func<T, bool> predicate)
        where T : struct 
    {
        if (entries.Count() == 0)
        {
            return null;
        }
        T first = entries.FirstOrDefault(predicate);
        return first.Equals(default(T)) ? null : first;
    }
}