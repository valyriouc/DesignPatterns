
namespace QuickNote.Core.Storage;

/// <summary>
/// Builder pattern in action
/// </summary>
public class MdWriterBuilder {
    
    private MarkdownWriter Writer { get; set;}

    public MdWriterBuilder() {
        Writer = new MarkdownWriter();
    }

    public MdWriterBuilder WithCheck() {
        Writer.AddModule(MdSyntax.Check, (MarkdownNode node) => {
            if (node.Identifier != MdSyntax.Check) {
                return null;
            }

            if (!node.TryConvert<bool>(out bool isChecked)) {
                return null;
            }

            return isChecked ? "[X]" : "[ ]";
        });

        return this;
    }

    public MdWriterBuilder WithName() {
        Writer.AddModule(MdSyntax.Name, (MarkdownNode node) => {
            if (node.Identifier != MdSyntax.Name) {
                return null;
            }

            if (!node.TryConvert<string>(out string? name)) {
                return null;
            }

            return $"{{{name}}}";
        });

        return this;
    }

    public MdWriterBuilder WithAppointment() {
        Writer.AddModule(MdSyntax.Appointment, (MarkdownNode node) => {
            if (node.Identifier != MdSyntax.Appointment) {
                return null;
            }
            if (!node.TryConvert<bool>(out bool isAppointment)) {
                return null;
            }

            return $"{{{isAppointment}}}";
        });

        return this;
    }

    public MdWriterBuilder WithEndDate() {
        Writer.AddModule(MdSyntax.Enddate, (MarkdownNode node) => {
            if (node.Identifier != MdSyntax.Enddate) {
                return null;
            }
            if (!node.TryConvert<DateTime>(out DateTime endDate)) {
                return null;
            }

            return $"{{{endDate}}}";
        });

        return this;
    }

    public MarkdownWriter Build() {
        MarkdownWriter writer = Writer;
        Writer = new MarkdownWriter();
        return writer;
    }
}