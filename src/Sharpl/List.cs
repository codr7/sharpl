using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Sharpl;

public static class List
{
    public static void Drop<T>(this List<T> items, int n) => items.RemoveRange(items.Count - n, n);

    public static T Peek<T>(this List<T> items, int offset = 0) => items[items.Count - 1 - offset];

    public static T Pop<T>(this List<T> items)
    {
        var i = items.Count - 1;
        var v = items[i];
        items.RemoveAt(i);
        return v;
    }

    public static void Push<T>(this List<T> items, T it) => items.Add(it);

    public static void Push<T>(this Stack items, Type<T> type, T data) where T : notnull =>
        items.Push(Value.Make(type, data));

    public static void Reverse(this Stack items, int n) => items.Reverse(items.Count - n, n);

    public static string ToString<T>(List<T> items)
    {
        if (items is null) { return ""; }
        var res = new StringBuilder();
        res.Append('[');

        for (var i = 0; i < items.Count; i++)
        {
            if (i > 0) { res.Append(' '); }
            var v = items[i];
            if (v is not null) { res.Append(v.ToString()); }
        }

        res.Append(']');
        return res.ToString();
    }

    public static bool TryPeek<T>(this List<T> items, [MaybeNullWhen(false)] out T? value)
    {
        if (items.Count > 0)
        {
            var i = items.Count - 1;
            value = items[i];
            return true;
        }

        value = default;
        return false;
    }

    public static bool TryPop<T>(this List<T> items, [MaybeNullWhen(false)] out T? value)
    {
        if (items.Count > 0)
        {
            var i = items.Count - 1;
            value = items[i];
            items.RemoveAt(i);
            return true;
        }

        value = default;
        return false;
    }

    public static void Trunc<T>(this List<T> items, int n) =>
        items.RemoveRange(n, items.Count - n);
}