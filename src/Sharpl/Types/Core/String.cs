
namespace Sharpl.Types.Core;

using System.Text;

public class StringType : ComparableType<string>
{
    public StringType(string name) : base(name) { }

    public override bool Bool(Value value) {
        return value.Cast(this).Length != 0;
    }
    
    public override void Dump(Value value, StringBuilder result)
    {
        result.Append('"');
        result.Append(value.Data);
        result.Append('"');
    }

    public override void Say(Value value, StringBuilder result)
    {
        result.Append(value.Data);
    }
}
