using System.Drawing;
using System.Text;

namespace Sharpl.Types.Core;

public class ColorType : Type<Color>
{
    public ColorType(string name) : base(name) { }


    public override void Dump(Value value, StringBuilder result)
    {
        var c = value.CastUnbox(this);
        result.Append($"(Color {c.R} {c.G} {c.B} {c.A})");
    }
}
