namespace Sharpl.Types.Core;

public class ComparableType<T>(string name) : Type<T>(name), ComparableTrait
    where T : IComparable<T>
{
    public Order Compare(Value left, Value right)
    {
        var lv = left.CastSlow(this);
        var rv = right.CastSlow(this);
        return ComparableTrait.IntOrder(lv.CompareTo(rv));
    }
}