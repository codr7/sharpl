using System.Collections;
using System.Text;

namespace Sharpl;

public class SList<T> : IEnumerable<T>, IList<T>
{
    private readonly List<T> items = new List<T>();

    public T this[int i]
    {
        get => items[i];
        set => items[i] = value;
    }

    public int Count { get { return items.Count; } }

    public void Add(T item)
    {
        items.Add(item);
    }

    public void Clear()
    {
        items.Clear();
    }

    public bool Contains(T it)
    {
        return items.Contains(it);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        items.CopyTo(array, arrayIndex);
    }

    public void Drop(int n)
    {
        items.RemoveRange(items.Count - n, n);
    }

    public bool Empty { get { return items.Count == 0; } }

    public IEnumerator<T> GetEnumerator()
    {
        return items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return items.GetEnumerator();
    }

    public int IndexOf(T it)
    {
        return items.IndexOf(it);
    }

    public void Insert(int i, T it)
    {
        items.Insert(i, it);
    }

    public bool IsReadOnly => throw new NotImplementedException();

    public T Peek(int offset = 0)
    {
        return items[items.Count - 1 - offset];
    }

    public T Pop()
    {
        var i = items.Count - 1;
        var v = items[i];
        items.RemoveAt(i);
        return v;
    }

    public void Push(T it)
    {
        items.Add(it);
    }

    public bool Remove(T item)
    {
        return items.Remove(item);
    }

    public void RemoveAt(int i)
    {
        items.RemoveAt(i);
    }

    public void Reverse(int i, int n)
    {
        items.Reverse(i, n);
    }

    public override string ToString()
    {
        if (items is null)
        {
            return "";
        }

        var res = new StringBuilder();
        res.Append('[');

        for (var i = 0; i < items.Count; i++)
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
        items.RemoveRange(n, items.Count - n);
    }
}