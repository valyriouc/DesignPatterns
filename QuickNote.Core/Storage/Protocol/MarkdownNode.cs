
namespace QuickNote.Core.Storage;

public struct MarkdownNode : IEquatable<MarkdownNode>
{

    public MdSyntax Identifier;

    private string? Content { get; }

    public MarkdownNode() {
        Identifier = MdSyntax.Finished;
    }

    public MarkdownNode(MdSyntax syntax, string content) {
        Identifier = syntax;
        Content = content;
    }

    public bool TryConvert<TType>(out TType? value) 
        where TType : IParsable<TType> {
        value = default(TType);
        if (Identifier is MdSyntax.Finished) return false;
        return TType.TryParse(Content, null, out value);
    }

    public bool Equals(MarkdownNode other)
    {
        return this.Identifier == other.Identifier && this.Content == other.Content;
    }
}