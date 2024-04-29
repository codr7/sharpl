public class Stack<T>
{
    private readonly int cap;
    private T[] items;
    private int len = 0;

    public T this[int i] => Get(i);

    public int Len { get { return len; } }

    public Stack(int cap)
    {
        this.cap = cap;
        items = new T[cap];
    }

    public void Drop(int n)
    {
        len -= n;
    }

    public T Get(int i)
    {
        return items[i];
    }

    public T Peek()
    {
        return items[len - 1];
    }

    public T Pop()
    {
        len--;
        return items[len];
    }

    public void Push(T it)
    {
        items[len] = it;
        len++;
    }
}