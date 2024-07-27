namespace Sharpl.Types.Core;

using System.Text;

public class FormType : Type<(Form, int)>
{
    public FormType(string name) : base(name) { }

    public override void Dump(Value value, StringBuilder result)
    {
        var (f, d) = value.Cast(this);
        for (var i = 0; i < d; i++) { result.Append('\''); }
        result.Append(f);
    }
    public override bool Equals(Value left, Value right)
    {
        var (lf, ld) = left.Cast(this);
        var (rf, rd) = right.Cast(this);
        return lf.Equals(lf) && ld == rd;
    }
}