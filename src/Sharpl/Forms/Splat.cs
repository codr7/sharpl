namespace Sharpl.Forms;

public class Splat : Form
{
    public readonly Form Target;

    public Splat(Loc loc, Form target) : base(loc)
    {
        Target = target;
    }

    public override void Emit(VM vm, Queue args)
    {
        vm.Emit(Target);
        vm.Emit(Ops.Splat.Make(Loc));
    }

    public override bool Equals(Form other) => (other is Splat f) ? f.Target.Equals(Target) : false;
    public override bool IsSplat => true;
    public override Form Quote(Loc loc, VM vm) => new Splat(loc, Target.Quote(loc, vm));
    public override string Dump(VM vm) => $"{Target.Dump(vm)}*";
    public override Form Unquote(Loc loc, VM vm) => new Splat(loc, Target);
}