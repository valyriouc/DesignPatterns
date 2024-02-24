
namespace QuickNote.Core.Storage;

public struct MarkdownNode {

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
}