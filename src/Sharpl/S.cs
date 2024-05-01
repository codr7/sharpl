namespace Sharpl;

public class S : ArrayStack<Value>
{
    public S(int cap) : base(cap) { }
    public void Push<T>(Type<T> type, T data) where T: notnull
    {
        this.Push(Value.Make(type, data));
    }
}