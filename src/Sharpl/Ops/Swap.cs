namespace Sharpl.Ops;

public class Swap : Op
{
    public static Op Make(Loc loc) => new Swap(loc);
    public readonly Loc Loc;
    public Swap(Loc loc): base(OpCode.Swap)
    {
        Loc = loc;
    }

    public override string Dump(VM vm) => $"Swap {Loc}";
}