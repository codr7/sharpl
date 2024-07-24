namespace Sharpl.Types.Core;

using System.Text;

public class IterType : Type<Iter>, IterTrait
{
    public IterType(string name) : base(name) { }

    public override void Dump(Value value, StringBuilder result)
    {
        result.Append($"{value.Cast(this)}");
    }

    public Iter CreateIter(Value target)
    {
        return target.Cast(this);
    }
}