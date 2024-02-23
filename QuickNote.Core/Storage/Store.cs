namespace QuickNote.Core.Storage;

/// <summary>
/// Represents the complete set of todo lists in a single directory 
/// </summary>
internal class Store {
    
    public HashSet<TodoFile> MarkdownFiles { get; init; }

    private Store(IEnumerable<TodoFile> files) {
        MarkdownFiles = new(files);
    }

    public static Store FromFileSystem(string basepath) {
        if (string.IsNullOrWhiteSpace(basepath)) {
            throw new ArgumentNullException(nameof(basepath));
        }

        IEnumerable<TodoFile> files = Directory
            .EnumerateFiles(basepath)
            .Where(x => x.Substring(x.IndexOf(".") + 1) == "md")
            .Select(x => new TodoFile(x));

        return new Store(files);
    }
}