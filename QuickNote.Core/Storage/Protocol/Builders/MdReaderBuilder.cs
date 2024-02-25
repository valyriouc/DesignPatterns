
namespace QuickNote.Core.Storage;

/// <summary>
/// Builder pattern in action
/// </summary>
public class MdReaderBuilder {
    
    private MarkdownReader Reader { get; set;}

    public MdReaderBuilder() {
        Reader = new();
    }

    public MdReaderBuilder WithCheck() {
        Reader.AddModule(MdSyntax.Check, (string content) => 
            new MarkdownNode(MdSyntax.Check, (content == "[X]").ToString()));
        return this;
    }

    public MdReaderBuilder WithName() {
        Reader.AddModule(MdSyntax.Name, (string content) => 
            new MarkdownNode(MdSyntax.Name, content.Substring(1, content.Length - 2)));
        return this;
    }

    public MdReaderBuilder WithAppointment() {
        Reader.AddModule(MdSyntax.Appointment, (string content) => 
            new MarkdownNode(MdSyntax.Appointment, (content.Substring(1, content.Length - 1) == "true").ToString()));
        return this;
    }

    public MdReaderBuilder WithEndDate() {
        Reader.AddModule(MdSyntax.Enddate, (string content) => 
            new MarkdownNode(MdSyntax.Appointment, content.Substring(1, content.Length - 1)));
        return this;
    }
    
    public MarkdownReader Build() {
        MarkdownReader reader = Reader;
        Reader = new MarkdownReader();
        return reader;
    }
}