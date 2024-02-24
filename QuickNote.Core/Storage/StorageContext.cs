using System.Collections;

namespace QuickNote.Core.Storage;

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

    public async IAsyncEnumerable<T> GetCurrentAsync<T>() 
        where T : IMarkdownReadable<T>{
        
        TodoFile? file = Store.MarkdownFiles
            .FirstOrDefault(x => x.Identifier.Date == DateTime.Now.Date);

        if (file is null) {
            yield break;
        }

        foreach (string? line in await file.Value.ReadLines) {
            MarkdownReader reader = Reader.Copy();
            IEnumerable<MarkdownNode> nodes = reader.Read(line);
            yield return T.Read(nodes);
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