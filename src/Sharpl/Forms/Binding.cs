namespace Sharpl.Forms;

public class Binding : Form
{
    public readonly Register Register;
    public Binding(Register reg, Loc loc) : base(loc)
    {
        Register = reg;
    }

    public override void Emit(VM vm, Queue args, Register result) =>
        vm.Emit(Ops.UnquoteRegister.Make(Register, Loc));

    public override bool Equals(Form other) => 
        other is Binding b && b.Register.Equals(Register);
    public override string Dump(VM vm) => $"{Register}";
}