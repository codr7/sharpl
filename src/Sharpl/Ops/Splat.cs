namespace Sharpl.Ops;

public class Splat : Op
{
    public static Op Make(Register target, Register result, Loc loc) => new Splat(target, result, loc);
    
    public readonly Register Target;
    public readonly Register Result;
    public readonly Loc Loc;
    
    public Splat(Register target, Register result, Loc loc)
    {
        Target = target;
        Result = result;
        Loc = loc;
    }

    public OpCode Code => OpCode.Splat;
    public string Dump(VM vm) => $"Splat {Target} {Result} {Loc}";
}