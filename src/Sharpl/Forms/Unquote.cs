namespace Sharpl.Forms;

public class UnquoteForm : Form
{
    public readonly Form Target;

    public UnquoteForm(Loc loc, Form target) : base(loc)
    {
        Target = target;
    }

    public override void CollectIds(HashSet<string> result) => Target.CollectIds(result);
    public override void Emit(VM vm, Queue args) => args.PushFirst(Target.Unquote(Loc, vm));
    public override bool Equals(Form other) => (other is UnquoteForm f) ? f.Target.Equals(Target) : false;

    public override bool Expand(VM vm, Queue args)
    {
        var result = Target.Expand(vm, args);
        args.Push(new UnquoteForm(Loc, args.PopLast()));
        return result;
    }

    public override bool IsSplat => Target.IsSplat;
    public override Form Quote(Loc loc, VM vm) => Target.Unquote(loc, vm);
    public override string Dump(VM vm) => $",{Target.Dump(vm)}";
    public override Form Unquote(Loc loc, VM vm) => new UnquoteForm(loc, this);
}