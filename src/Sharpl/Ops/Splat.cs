namespace Sharpl.Ops;

public class Splat : Op
{
    public static Op Make(Loc loc) => new Splat(loc);
    public readonly Loc Loc;
    public Splat(Loc loc)
    {
        Loc = loc;
    }

    public OpCode Code => OpCode.Splat;
    public string Dump(VM vm) => $"Splat {Loc}";
}