namespace Sharpl.Types.Core;

using System.Text;

public class NilType : Type<bool>
{
    public NilType(string name) : base(name) { }

    
    public override bool Bool(Value value) {
        return false;
    }

    public override void Dump(Value value, StringBuilder result)
    {
        result.Append('_');
    }  
}
