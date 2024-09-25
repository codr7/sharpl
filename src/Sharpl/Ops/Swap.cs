namespace Sharpl.Ops;

public class Swap : Op
{
    public static Op Make(Loc loc) => new Swap(loc);
    public readonly Loc Loc;
    public Swap(Loc loc)
    {
        Loc = loc;
    }

    public OpCode Code => OpCode.Swap;
    public string Dump(VM vm) => $"Swap {Loc}";
}