namespace Sharpl.Forms;

public class Binding : Form
{
    public readonly Register Register;
    public Binding(Loc loc, Register reg) : base(loc)
    {
        Register = reg;
    }

    public override void Emit(VM vm, Queue args) =>
        vm.Emit(Ops.UnquoteRegister.Make(Loc, Register));

    public override bool Equals(Form other) => other is Binding b && b.Register.Equals(Register);
    public override string Dump(VM vm) => $"{Register}";
}