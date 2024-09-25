namespace Sharpl.Ops;

public class CopyRegister : Op
{
    public static Op Make(Register from, Register to) => new CopyRegister(from, to);

    public readonly Register From;
    public readonly Register To;

    public CopyRegister(Register from, Register to): base(OpCode.CopyRegister)
    {
        From = from;
        To = to;
    }

    public override string Dump(VM vm) =>
        $"CopyRegister {From} {To}";
}