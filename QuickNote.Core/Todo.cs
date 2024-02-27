using QuickNote.Core.Storage;

namespace QuickNote.Core;

public interface IMarkdownReadable<T> {

    public abstract static T Read(IEnumerable<MarkdownNode> nodes);
}

public interface IMarkdownWriteable<T> {

    public IEnumerable<MarkdownNode> Write();
}

public struct Todo : IMarkdownReadable<Todo>, IMarkdownWriteable<Todo>{

    private static int counter = 0;

    public int Id { get; init; }

    public bool IsFinished { get; set; }

    public string Name { get; set; }

    public bool? IsAppointment { get; set; } = null;

    public DateTime? EndDate { get; set; } = null;

    public Todo(bool isFinished, string name, bool isAppoint, DateTime? endDate) {
        Id = ++counter;
        IsFinished = isFinished;
        Name = name;
        IsAppointment = isAppoint;
        EndDate = endDate;
    }

    public static Todo Read(IEnumerable<MarkdownNode> nodes)
    {
        Todo todo = new();
        IEnumerator<MarkdownNode> enumerator = nodes.GetEnumerator();
        while(enumerator.MoveNext()) {
            MarkdownNode node = enumerator.Current;
            switch (node.Identifier) {
                case MdSyntax.Check:
                    if (!node.TryConvert<bool>(out bool isChecked)) {
                        throw new Exception($"Could not parse {nameof(todo.IsFinished)}!");
                    }
                    todo.IsFinished = isChecked;
                    break;
                case MdSyntax.Name:
                    if (!node.TryConvert<string>(out string? name)) {
                        throw new Exception($"Could not parse {nameof(todo.Name)}!");
                    }
                    todo.Name = name!;
                    break;
                case MdSyntax.Appointment:
                    if (!node.TryConvert<bool>(out bool isAppointment)) {
                        throw new Exception($"Could not parse {nameof(todo.IsAppointment)}!");
                    }
                    todo.IsAppointment = isAppointment;
                    break;
                case MdSyntax.Enddate:
                    if (!node.TryConvert<DateTime>(out DateTime endDate)) {
                        throw new Exception($"Could not parse {nameof(todo.IsAppointment)}!");
                    }
                    todo.EndDate = endDate;
                    break;
                default:
                    break;
            }
        }
        
        return todo;
    }

    public IEnumerable<MarkdownNode> Write()
    {
        yield return new MarkdownNode(MdSyntax.Check, IsFinished.ToString());
        yield return new MarkdownNode(MdSyntax.Name, Name);
        if (this.IsAppointment is not null)
        {
            yield return new MarkdownNode(MdSyntax.Appointment, IsAppointment!.ToString()!);
        }
        if (EndDate is not null) {
            yield return new MarkdownNode(MdSyntax.Enddate, EndDate!.ToString()!);
        }
        yield return new MarkdownNode();
    }
}