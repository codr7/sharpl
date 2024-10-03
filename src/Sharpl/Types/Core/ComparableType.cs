namespace Sharpl.Types.Core;

public class ComparableType<T>(string name, AnyType[] parents) : 
    Type<T>(name, parents), ComparableTrait where T : IComparable<T>
{
    public Order Compare(Value left, Value right)
    {
        var lv = left.CastSlow(this);
        var rv = right.CastSlow(this);
        return ComparableTrait.IntOrder(lv.CompareTo(rv));
    }
}