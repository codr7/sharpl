namespace Sharpl.Ops;

public class Swap : Op
{
    public static Op Make(Register x, Register y) => new Swap(x, y);

    public readonly Register X;
    public readonly Register Y;

    public Swap(Register x, Register y)
    {
        X = x;
        Y = y;
    }

    public OpCode Code => OpCode.Swap;
    public string Dump(VM vm) => $"Swap {X} {Y}";
}