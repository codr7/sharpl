namespace Sharpl.Types.Core;

using System.Text;
using Sharpl.Forms;

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

    public override Form Unquote(Value value, Loc loc, VM vm)
    {
        var (f, d) = value.Cast(this);
        Console.WriteLine("FORM UNQUOTE" + f + " " + d);

        if (d > 1) { return new Quote(loc, f, d - 1); }

        if (d == 1)
        {
            if (f is Quote q)
            {
                if (q.Depth > 1) { return new Quote(loc, q.Target, q.Depth - 1); }
                if (q.Depth == 1) { return q.Target; }
                throw new EvalError(loc, $"Invalid quote: {q}");
            }

            return f;
        }
        
        throw new EvalError(loc, $"Not quoted: {value}");
    }
}