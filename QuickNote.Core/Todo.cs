using QuickNote.Core.Storage;

namespace QuickNote.Core;

public interface IMarkdownReadable<T> {

    public abstract static T Read(IEnumerable<MarkdownNode> nodes);
}

public interface IMarkdownWriteable<T> {

    public IEnumerable<MarkdownNode> Write();
}

public struct Todo : IMarkdownReadable<Todo>, IMarkdownWriteable<Todo>{

    public bool IsFinished { get; set; }

    public string Name { get; set; }

    public bool IsAppointment { get; set;}

    public DateTime? EndDate { get; set; }

    public Todo(bool isFinished, string name, bool isAppoint, DateTime? endDate) {
        IsFinished = isFinished;
        Name = name;
        IsAppointment = isAppoint;
        EndDate = endDate;
    }

    public static Todo Read(IEnumerable<MarkdownNode> nodes)
    {
        Todo todo = new();

        bool done = false;
        IEnumerator<MarkdownNode> enumerator = nodes.GetEnumerator();
        while(!done) {
            MarkdownNode node = enumerator.Current;
            if (node.Equals(default(MarkdownNode)))
            {
                break;
            }
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
                    done = true;
                    break;
            }
            enumerator.MoveNext();
        }
        
        return todo;
    }

    public IEnumerable<MarkdownNode> Write()
    {
        yield return new MarkdownNode(MdSyntax.Check, IsFinished.ToString());
        yield return new MarkdownNode(MdSyntax.Check, Name);
        yield return new MarkdownNode(MdSyntax.Check, IsAppointment.ToString());
        if (EndDate is not null) {
            yield return new MarkdownNode(MdSyntax.Check, EndDate!.ToString());
        }
        yield return new MarkdownNode();
    }
}