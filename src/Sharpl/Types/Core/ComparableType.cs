namespace Sharpl.Types.Core;

public class ComparableType<T> : Type<T>, ComparableTrait where T : IComparable<T>
{
    public ComparableType(string name) : base(name)
    {
    }

    public Order Compare(Value left, Value right)
    {
        var lv = left.Cast(this);
        var rv = right.Cast(this);
        return ComparableTrait.IntOrder(lv.CompareTo(rv));
    }
}