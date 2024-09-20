namespace Sharpl.Forms;

public class UnquoteForm : Form
{
    public readonly Form Target;

    public UnquoteForm(Form target, Loc loc) : base(loc)
    {
        Target = target;
    }

    public override void CollectIds(HashSet<string> result) => Target.CollectIds(result);
    public override void Emit(VM vm, Queue args) => args.PushFirst(Target.Unquote(vm, Loc));
    public override bool Equals(Form other) => (other is UnquoteForm f) ? f.Target.Equals(Target) : false;

    public override bool Expand(VM vm, Queue args)
    {
        var result = Target.Expand(vm, args);
        args.Push(new UnquoteForm(args.PopLast(), Loc));
        return result;
    }

    public override bool IsSplat => Target.IsSplat;
    public override Form Quote(VM vm, Loc loc) => Target.Unquote(vm, loc);
    public override string Dump(VM vm) => $",{Target.Dump(vm)}";
    public override Form Unquote(VM vm, Loc loc) => new UnquoteForm(this, loc);
}