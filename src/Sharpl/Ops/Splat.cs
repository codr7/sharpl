namespace Sharpl.Ops;

public class Splat : Op
{
    public static Op Make(Register target, Loc loc) => new Splat(target, loc);
    
    public readonly Register Target;
    public readonly Loc Loc;
    
    public Splat(Register target, Loc loc)
    {
        Target = target;
        Loc = loc;
    }

    public OpCode Code => OpCode.Splat;
    public string Dump(VM vm) => $"Splat {Target} {Loc}";
}