
using System.Text;

namespace QuickNote.Core.Storage;

internal struct TodoFile : IEquatable<TodoFile> {

    public string Fullpath { get; }

    public DateTime Identifier { get; }

    public Task<string> ReadOp => File.ReadAllTextAsync(Fullpath);

    internal TodoFile(string path) {
        Fullpath = path;
        Identifier = DateFromFilePath(path);
    }

    private static DateTime DateFromFilePath(string path) {
        string filename = Path.GetFileNameWithoutExtension(path);
        return DateTime.Parse(filename);
    }

    public async Task AppendAsync(Func<IAsyncEnumerable<string>> contentProvider) {
        StringBuilder sb = new StringBuilder();
        await foreach (string line in contentProvider()) {
            sb.AppendLine(line);
        }
        using StreamWriter writer = File.AppendText(Fullpath);
        writer.Write(sb.ToString());
    }

    public static TodoFile CreateFrom(string basepath, DateTime date) {
        string filename = date.ToString("yyyy-MM-dd");
        string path = Path.Combine(basepath, $"{filename}.md");
        File.Create(path);
        return new TodoFile(path);
    }

    public bool TryDelete() {
        try {
            File.Delete(this.Fullpath);
            return true;
        }
        catch (Exception) {
            return false;
        } 
    }

    public bool Equals(TodoFile other) => 
        this.Identifier.Date == other.Identifier.Date;
}