
namespace QuickNote.Core.Helper;

public interface IFactoryCreatable<T> {
    public static abstract T Create();
}