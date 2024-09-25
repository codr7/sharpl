namespace Sharpl.Ops;

public class Splat : Op
{
    public static Op Make(Loc loc) => new Splat(loc);
    public readonly Loc Loc;
    public Splat(Loc loc): base(OpCode.Splat)
    {
        Loc = loc;
    }

    public override string Dump(VM vm) => $"Splat {Loc}";
}