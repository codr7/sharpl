using System.Text;

namespace Sharpl.Types.Core;

public class IntType : Type<int>
{

    public IntType(string name) : base(name) { }

    public override void Dump(Value value, StringBuilder result)
    {
        result.Append(value.ToString());
    }
}