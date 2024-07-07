namespace Sharpl.Types.Core;

public class IntType : ComparableType<int>, IterableTrait
{
    public IntType(string name) : base(name) { }

    public override bool Bool(Value value)
    {
        return value.Cast(this) != 0;
    }

    public Iter CreateIter(Value target)
    {
        return new Iters.Core.Range(0, target.Cast(this));
    }
}