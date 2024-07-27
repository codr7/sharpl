namespace Sharpl.Forms;

public class Splat : Form
{
    public readonly Form Target;

    public Splat(Loc loc, Form target) : base(loc)
    {
        Target = target;
    }

    public override void Emit(VM vm, Queue args, int quoted)
    {   
        vm.Emit(Target, 0);
        vm.Emit(Ops.Splat.Make(Loc));
    }

    public override bool Equals(Form other)
    {
        return (other is Splat f) ? f.Target.Equals(Target) : false;
    }

    public override bool IsSplat => true;

    public override string ToString() {
        return $"{Target}*";
    }
}