using QuickNote.Core.Helper;

namespace QuickNote.Core.Storage;

public enum MdSyntax {
    Check = 0,
    Name = 1,
    Appointment = 2,
    Enddate = 3,
    Finished = 100
}

internal delegate MarkdownNode? MdReaderDelegate(string content);

internal sealed class MarkdownReader : IPrototypable<MarkdownReader>
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
        if (start is null) {
            throw new Exception("Invalid markdown!");
        }
        yield return (MarkdownNode)start;

        int dctr = 1;
        int mctr = 1;
        while (true) {
            MdReaderDelegate del = Modules[(MdSyntax)mctr];
            MarkdownNode? node = del(data[dctr]);
            if (node is null) {
                mctr += 1;
                continue;
            }
            MarkdownNode notNull = (MarkdownNode)node;
            if (node!.Value.Identifier is MdSyntax.Finished) {
                yield return notNull;
                yield break;
            }
            yield return notNull;
            dctr += 1;
            mctr += 1;
        }
    }

    public MarkdownReader Copy()
    {
        MarkdownReader reader = new();

        reader.Modules = this.Modules;

        return reader;
    }
}