
using System.Reflection;
using System.Runtime.CompilerServices;
using QuickNote.Core.Helper;

namespace QuickNote.Core.Storage;

internal enum MarkdownModule {
    Check = 0,
    Name = 1,
    Appointment = 2,
    Enddate = 3
}

internal delegate MarkdownNode? ModuleDelegate(string content);

internal sealed class MarkdownReader : IPrototypable<MarkdownReader>
{
    private Dictionary<MarkdownModule, ModuleDelegate> Modules { get; set;}

    public MarkdownReader() {
        Modules = new Dictionary<MarkdownModule, ModuleDelegate>();
    }

    public bool AddModule(MarkdownModule identifier, ModuleDelegate module) {
        if (Modules.ContainsKey(identifier)) {
            return false;
        }

        Modules.Add(identifier, module);
        return true;
    }

    public bool RemoveModule(MarkdownModule module) => Modules.Remove(module);

    public IEnumerable<MarkdownNode> Read(string line) {
        if (string.IsNullOrWhiteSpace(line)) {
            throw new Exception("Invalid markdown!");
        }

        string[] data = line.Split(" ");
        MarkdownNode? start = Modules[MarkdownModule.Check](data[0]);
        if (start is null) {
            throw new Exception("Invalid markdown!");
        }
        yield return (MarkdownNode)start;

        int dctr = 1;
        int mctr = 1;
        while (true) {
            ModuleDelegate del = Modules[(MarkdownModule)mctr];
            MarkdownNode? node = del(data[dctr]);
            if (node is null) {
                mctr += 1;
                continue;
            }
            yield return (MarkdownNode)node;
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