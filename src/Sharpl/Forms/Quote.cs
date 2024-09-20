using System.Text;

namespace Sharpl.Forms;

public class QuoteForm : Form
{
    public readonly Form Target;

    public QuoteForm(Form target, Loc loc) : base(loc)
    {
        Target = target;
    }

    public override void CollectIds(HashSet<string> result) => Target.CollectIds(result);
    public override void Emit(VM vm, Queue args) => args.PushFirst(Target.Quote(vm, Loc));
    public override void EmitCall(VM vm, Queue args) => Target.Quote(vm, Loc).EmitCall(vm, args);
    public override bool Equals(Form other) => (other is QuoteForm f) && f.Target.Equals(Target);

    public override bool Expand(VM vm, Queue args)
    {
        var result = Target.Expand(vm, args);
        args.Push(new QuoteForm(args.PopLast(), Loc));
        return result;
    }

    public override bool IsSplat => Target.IsSplat;
    public override Form Quote(VM vm, Loc loc) => new QuoteForm(this, loc);
    public override string Dump(VM vm) => $"{Target.Dump(vm)}";
    public override Form Unquote(VM vm, Loc loc) => Target;
}