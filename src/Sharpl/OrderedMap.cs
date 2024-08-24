namespace Sharpl;

public class OrderedMap<K, V> where K : IComparable<K>
{
    private readonly List<(K, V)> items;

    public OrderedMap((K, V)[] items)
    {
        this.items = new List<(K, V)>(items);
    }

    public OrderedMap() : this([]) { }

    public V? this[K key]
    {
        get => Get(key);
        set => Set(key, value);
    }

    public bool ContainsKey(K key)
    {
        var (_, ok) = Find(key);
        return ok;
    }

    public int Count { get => items.Count; }

    public void Delete(int i) => items.RemoveAt(i);

    public (int, bool) Find(K key)
    {
        var min = 0;
        var max = items.Count;

        while (min < max)
        {
            var i = (min + max) / 2;
            var it = items[i];
            var cres = key.CompareTo(it.Item1);
            if (cres < 0) { max = i; }
            else if (cres > 0) { min = i + 1; }
            else { return (i, true); }
        }

        return (max, false);
    }

    public V? Get(K key)
    {
        var (i, ok) = Find(key);
        return ok ? items[i].Item2 : default;
    }

    public IEnumerator<(K, V)> GetEnumerator() => items.AsEnumerable().GetEnumerator();

    public int IndexOf(K key)
    {
        var (i, ok) = Find(key);
        return ok ? i : -1;
    }

    public void Insert(int i, K key, V value) => items.Insert(i, (key, value));
    public (K, V)[] Items => items.ToArray();

    public V? Remove(K key)
    {
        var (i, ok) = Find(key);
        if (!ok) { return default; }
        var v = items[i].Item2;
        Delete(i);
        return v;
    }

    public V? Set(K key, V? value)
    {
        var (i, ok) = Find(key);

        if (value is V v)
        {
            if (ok)
            {
                var pv = items[i].Item2;
                items[i] = (key, value);
                return pv;
            }

            Insert(i, key, v);
            return default;
        }

        if (ok)
        {
            var pv = items[i].Item2;
            Delete(i);
            return pv;
        }

        return default;
    }
}