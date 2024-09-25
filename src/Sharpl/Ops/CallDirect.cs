namespace Sharpl.Ops;

public class CallDirect : Op
{
    public static Op Make(Value Target, int arity, bool splat, int registerCount, Loc loc) =>
        new CallDirect(Target, arity, splat, registerCount, loc);

    public readonly Loc Loc;
    public readonly Value Target;
    public readonly int Arity;
    public readonly bool Splat;
    public readonly int RegisterCount;

    public CallDirect(Value target, int arity, bool splat, int registerCount, Loc loc)
    {
        Target = target;
        Arity = arity;
        Splat = splat;
        RegisterCount = registerCount;
        Loc = loc;
    }

    public OpCode Code => OpCode.CallDirect;

    public string Dump(VM vm) =>
        $"CallDirect {Loc} {Target} {Arity} {Splat} {RegisterCount}";
}