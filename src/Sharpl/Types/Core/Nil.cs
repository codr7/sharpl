namespace Sharpl.Types.Core;

using System.Text;

public class NilType : Type<bool>, IterableTrait
{
    public NilType(string name) : base(name) { }

    
    public override bool Bool(Value value) {
        return false;
    }

    public Iter CreateIter(Value target)
    {
        return Iters.Core.Nil.Instance;
    }

    public override void Dump(Value value, StringBuilder result)
    {
        result.Append('_');
    }  
}
