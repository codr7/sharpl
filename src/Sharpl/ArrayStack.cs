using System.Text;

namespace Sharpl;

public class ArrayStack<T>
{
    private readonly int cap;
    private readonly T[] items;
    private int len = 0;

    public T this[int i] => Get(i);

    public int Len { get { return len; } }

    public ArrayStack(int cap)
    {
        this.cap = cap;
        items = new T[cap];
    }

    public void Drop(int n)
    {
        len -= n;
    }

    public bool Empty { get { return len == 0; } }

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

    public void Reverse(int n) {
        Array.Reverse(items, Len - n, n);
    }

    public override string ToString()
    {
        if (items is null)
        {
            return "";
        }

        var res = new StringBuilder();
        res.Append('[');

        for (var i = 0; i < len; i++)
        {
            if (i > 0)
            {
                res.Append(' ');
            }

            var v = items[i];

            if (v is not null)
            {
                res.Append(v.ToString());
            }
        }

        res.Append(']');
        return res.ToString();
    }
}