namespace Sharpl.Types.Core;

using System.Text;

public class IntType : ComparableType<int>
{
    public IntType(string name) : base(name) { }

    public override bool Bool(Value value) {
        return value.Cast(this) != 0;
    }
}