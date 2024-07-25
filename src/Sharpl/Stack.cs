namespace Sharpl;

public class Stack : ArrayStack<Value>
{
    public Stack(int cap) : base(cap) { }
    public void Push<T>(Type<T> type, T data) where T: notnull
    {
        this.Push(Value.Make(type, data));
    }

    public void Reverse(int n)
    {
        Reverse(Count - n, n);
    }    
}