namespace Sharpl.Types.Core;

using System.Text;

public class IterType : Type<Iter>, IterTrait
{
    public IterType(string name) : base(name) { }

    public override void Dump(Value value, StringBuilder result)
    {
        result.Append($"Iter ${this}");
    }

    public Iter Iter(Value target)
    {
        return target.Cast(this);
    }
}