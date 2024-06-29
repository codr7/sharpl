using System.Collections;
using System.Dynamic;
using System.Text;

namespace Sharpl;

public class ArrayStack<T>: IEnumerable<T>
{
    private readonly int capacity;
    private readonly T[] items;
    private int count = 0;

    public T this[int i] {
      get => Get(i);
      set => Set(i, value);
    }

    public int Count { get { return count; } }

    public ArrayStack(int capacity)
    {
        this.capacity = capacity;
        items = new T[capacity];
    }

    public void Clear() {
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
        items[count] = it;
        count++;
    }

    public void Reverse(int n) {
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

    public void Trunc(int n) {
        count = n;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return items[0..count].AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return items[0..count].GetEnumerator();
    }
}