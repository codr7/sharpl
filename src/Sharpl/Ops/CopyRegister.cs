namespace Sharpl.Ops;

public class CopyRegister : Op
{
    public static Op Make(Register from, Register to) => new CopyRegister(from, to);

    public readonly Register From;
    public readonly Register To;

    public CopyRegister(Register from, Register to)
    {
        From = from;
        To = to;
    }

    public OpCode Code => OpCode.CopyRegister;
    public string Dump(VM vm) => $"CopyRegister {From} {To}";
}