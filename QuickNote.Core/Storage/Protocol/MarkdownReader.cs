using QuickNote.Core.Helper;

namespace QuickNote.Core.Storage;

public enum MdSyntax {
    Check = 0,
    Name = 1,
    Appointment = 2,
    Enddate = 3,
    Finished = 100
}

public delegate MarkdownNode? MdReaderDelegate(string content);

public sealed class MarkdownReader : IPrototypable<MarkdownReader>
{
    private Dictionary<MdSyntax, MdReaderDelegate> Modules { get; set;}

    public MarkdownReader() {
        Modules = new Dictionary<MdSyntax, MdReaderDelegate>();
    }

    public bool AddModule(MdSyntax identifier, MdReaderDelegate module) {
        if (Modules.ContainsKey(identifier)) {
            return false;
        }

        Modules.Add(identifier, module);
        return true;
    }

    public bool RemoveModule(MdSyntax module) => Modules.Remove(module);

    public IEnumerable<MarkdownNode> Read(string line) {

        if (string.IsNullOrWhiteSpace(line)) {
            yield break;
        }

        string[] data = line.Split(" ");
        MarkdownNode? start = Modules[MdSyntax.Check](data[0]);
        ThrowIfNull(start);
        yield return (MarkdownNode)start!;

        if (data.Length == 1)
        {
            throw new Exception("Name is required!");
        }

        MarkdownNode? name = Modules[MdSyntax.Name](data[1]);
        ThrowIfNull(name);
        yield return (MarkdownNode)name!;   

        if (data.Length == 2)
        {
            yield return new MarkdownNode();
            yield break;
        }

        if (Modules.ContainsKey(MdSyntax.Appointment))
        {
            MarkdownNode? appointment = Modules[MdSyntax.Appointment](data[2]);
            ThrowIfNull(appointment);
            yield return (MarkdownNode)appointment!;    
        }

        if (data.Length == 3)
        {
            yield return new MarkdownNode();
            yield break;
        }

        if (Modules.ContainsKey(MdSyntax.Enddate))
        {
            MarkdownNode? node = Modules[MdSyntax.Enddate](data[3]);
            ThrowIfNull(node);
            yield return (MarkdownNode)node!;
        }

        yield return new MarkdownNode();
        yield break;
    }

    private void ThrowIfNull(object? value)
    {
        if (value is null)
        {
            throw new Exception("Invalid markdown!");
        }
    }

    public MarkdownReader Copy()
    {
        MarkdownReader reader = new();

        reader.Modules = this.Modules;

        return reader;
    }
}