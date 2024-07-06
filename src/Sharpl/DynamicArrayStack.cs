using System.Collections;
using System.Text;

namespace Sharpl;

public class DynamicArrayStack<T> : IEnumerable<T>
{
    private int count = 0;
    private T[] items;

    public T this[int i]
    {
        get => Get(i);
        set => Set(i, value);
    }

    public int Count { get { return count; } }

    public DynamicArrayStack(int capacity)
    {
        items = new T[capacity];
    }

    public void Clear()
    {
        count = 0;
    }

    public void Drop(int n)
    {
        count -= n;
    }

    public bool Empty { get { return count == 0; } }

    public T Get(int i)
    {
        return items[i];
    }

    public void Delete(int i) {
        Array.ConstrainedCopy(items, i+1, items, i, count - i - 1);
        count--;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return items[0..count].AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return items[0..count].GetEnumerator();
    }

    public void Insert(int i, T it) {
        Reserve(1);

        if (i < count) {
            Array.ConstrainedCopy(items, i, items, i+1, count - i);
        }

        items[i] = it;
        count++;
    }

    public T[] Items { get => items; }
    
    public T Peek(int offset = 0)
    {
        return items[count - 1 - offset];
    }

    public T Pop()
    {
        count--;
        return items[count];
    }

    public void Push(T it)
    {
        Reserve(1);
        items[count] = it;
        count++;
    }

    public void Reserve(int n) {
        while (count + n <= items.Length)
        {
            Array.Resize(ref items, items.Length * 2);
        }
    }

    public void Reverse(int n)
    {
        Array.Reverse(items, Count - n, n);
    }

    public void Set(int i, T value)
    {
        items[i] = value;
    }

    public override string ToString()
    {
        if (items is null)
        {
            return "";
        }

        var res = new StringBuilder();
        res.Append('[');

        for (var i = 0; i < count; i++)
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

    public void Trunc(int n)
    {
        count = n;
    }
}