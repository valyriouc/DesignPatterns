
using System.Reflection;
using System.Text;
using QuickNote.Core.Helper;

namespace QuickNote.Core.Storage;

internal delegate string? MdWritingDelegate(MarkdownNode node);

internal sealed class MarkdownWriter : IPrototypable<MarkdownWriter> {

    private Dictionary<MdSyntax, MdWritingDelegate> Modules {get; set;}

    public MarkdownWriter() {
        Modules = new Dictionary<MdSyntax, MdWritingDelegate>();
    }

    public bool AddModule(MdSyntax identifier, MdWritingDelegate module) {
        if (Modules.ContainsKey(identifier)) {
            return false;
        }

        Modules.Add(identifier, module);
        return true;
    }

    public bool RemoveModule(MdSyntax identifier) => Modules.Remove(identifier);

    public string Write(IEnumerable<MarkdownNode> line) {
        if (line is null) {
            throw new ArgumentNullException(nameof(line));
        }

        StringBuilder sb = new StringBuilder();

        IEnumerator<MarkdownNode> enumerator = line.GetEnumerator();
        if (enumerator.Current.Identifier is not MdSyntax.Check) {
            throw new Exception("Expected markdown check node!");
        }
        
        string? start = Modules[MdSyntax.Check].Invoke(enumerator.Current);

        while (enumerator.MoveNext()) {
            MarkdownNode current = enumerator.Current;
            if (current.Identifier is MdSyntax.Finished)
                break;
            string? result = Modules[current.Identifier].Invoke(current);
            if (string.IsNullOrWhiteSpace(result))
            sb.Append($" {result}");
        }

        return sb.ToString();
    }

    public MarkdownWriter Copy()
    {
        MarkdownWriter writer = new();

        writer.Modules = this.Modules;

        return writer;
    }
}