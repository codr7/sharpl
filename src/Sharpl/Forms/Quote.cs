using System.Text;

namespace Sharpl.Forms;

public class QuoteForm : Form
{
    public readonly Form Target;
    public readonly int Depth;

    public QuoteForm(Loc loc, Form target) : base(loc)
    {
        Target = target;
    }

    public override void CollectIds(HashSet<string> result) => Target.CollectIds(result);
    public override void Emit(VM vm, Queue args) => args.PushFirst(Target.Quote(Loc, vm));
    public override bool Equals(Form other) => (other is QuoteForm f) && f.Target.Equals(Target);
    public override bool IsSplat => Target.IsSplat;
    public override Form Quote(Loc loc, VM vm) => new QuoteForm(loc, this);

    public override string ToString()
    {
        var buf = new StringBuilder();
        for (var i = 0; i < Depth; i++) { buf.Append('\''); }
        buf.Append(Target);
        return buf.ToString();
    }

    public override Form Unquote(Loc loc, VM vm) => Target.Unquote(loc, vm);
}