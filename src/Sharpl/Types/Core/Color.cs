namespace Sharpl.Types.Core;

using System.Drawing;
using System.Text;

public class ColorType : Type<Color>
{
    public ColorType(string name) : base(name) { }


    public override void Dump(Value value, StringBuilder result)
    {
        var c = value.Cast(this);
        result.Append($"(Color {c.R} {c.G} {c.B} {c.A})");
    }
}
