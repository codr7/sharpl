using System.Collections;
using System.Text;

namespace Sharpl;

public class ArrayStack<T> : IEnumerable<T>, IList<T>
{
    private readonly T[] items;
    private int count = 0;

    public T this[int i]
    {
        get => Get(i);
        set => Set(i, value);
    }

    public int Count { get { return count; } }

    public ArrayStack(int capacity)
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

    public bool IsReadOnly => throw new NotImplementedException();

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

    public IEnumerator<T> GetEnumerator()
    {
        return items[0..count].AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return items[0..count].GetEnumerator();
    }

    public int IndexOf(T item)
    {
        return Array.IndexOf(items, item);
    }

    public void Insert(int index, T item)
    {
        throw new NotImplementedException();
    }

    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
    }

    public void Add(T item)
    {
        Push(item);
    }

    public bool Contains(T item)
    {
        return Array.IndexOf(items, item) > -1;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        Array.Copy(items[arrayIndex..], array, count);
    }

    public bool Remove(T item)
    {
        if (count > 0)
        {
            Pop();
            return true;
        }

        return false;
    }
}