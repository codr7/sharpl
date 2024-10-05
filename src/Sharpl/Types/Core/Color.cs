using System.Drawing;
using System.Text;

namespace Sharpl.Types.Core;

public class ColorType(string name, AnyType[] parents) : Type<Color>(name, parents)
{
    public override void Dump(VM vm, Value value, StringBuilder result)
    {
        var c = value.CastUnbox(this);
        result.Append($"(Color {c.R} {c.G} {c.B} {c.A})");
    }
}
