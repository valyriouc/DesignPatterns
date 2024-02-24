
namespace QuickNote.Core.Storage;

internal struct MarkdownNode {

    public bool IsFinish { get; }

    public ValueType? Type { get; }

    public string? Content { get; }

    public MarkdownNode() {
        IsFinish = true;
    }

    public MarkdownNode(ValueType type, string content) {
        Type = type;
        Content = content;
    }

    public bool TryConvert<TType>(out TType? value) 
        where TType : IParsable<TType> {
        value = default(TType);
        if (IsFinish) return false;
        return TType.TryParse(Content, null, out value);
    }
}
