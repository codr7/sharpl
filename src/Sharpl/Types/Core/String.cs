
namespace Sharpl.Types.Core;

using System.Text;

public class StringType : Type<int>
{
    public StringType(string name) : base(name) { }

    public override void Dump(Value value, StringBuilder result)
    {
        result.Append('"');
        result.Append(value.Data);
        result.Append('"');
    }
}
