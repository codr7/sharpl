namespace Sharpl.Ops;

public class UnquoteRegister : Op
{
    public static Op Make(Register register, Loc loc) => new UnquoteRegister(register, loc);
    public readonly Register Register;
    public readonly Loc Loc;
    public UnquoteRegister(Register register, Loc loc)
    {
        Register = register;
        Loc = loc;
    }

    public OpCode Code => OpCode.UnquoteRegister;
    public string Dump(VM vm) => $"UnquoteRegister {Loc} {Register}";
}