
namespace QuickNote.Core.Helper;

public interface IFactoryCreatable<T> {
    
    public abstract T Create(Action<T> configure);

}