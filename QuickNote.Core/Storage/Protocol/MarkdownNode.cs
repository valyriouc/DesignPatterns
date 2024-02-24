
namespace QuickNote.Core.Storage;

internal struct MarkdownNode {

    public MdSyntax Identifier;

    private string? Content { get; }

    public MarkdownNode() {
        Identifier = MdSyntax.Finished;
    }

    private MarkdownNode(MdSyntax syntax, string content) {
        Identifier = syntax;
        Content = content;
    }

    public bool TryConvert<TType>(out TType? value) 
        where TType : IParsable<TType> {
        value = default(TType);
        if (Identifier is MdSyntax.Finished) return false;
        return TType.TryParse(Content, null, out value);
    }

    public static MarkdownNode Create<T>(
        MdSyntax ident,
        T value)
        where T : struct {
        Type type = typeof(T);
        if (!type.IsValueType) {
            throw new Exception("Expected value type!");
        }
        MarkdownNode node = new MarkdownNode(ident, value.ToString());

        return node;
    }
}