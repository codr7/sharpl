namespace Sharpl.Ops;

public class UnquoteRegister : Op
{
    public static Op Make(Register register, Loc loc) => new UnquoteRegister(register, loc);
    public readonly Register Register;
    public readonly Loc Loc;
    public UnquoteRegister(Register register, Loc loc): base(OpCode.UnquoteRegister)
    {
        Register = register;
        Loc = loc;
    }

    public override string Dump(VM vm) => $"UnquoteRegister {Loc} {Register}";
}