namespace Sharpl.Ops;

public class CallMethod : Op
{
    public static Op Make(Method target, int arity, bool splat, Register result, Loc loc) =>
        new CallMethod(target, arity, splat, result, loc);

    public readonly int Arity;
    public readonly Loc Loc;
    public readonly Register Result;
    public readonly bool Splat;
    public readonly Method Target;

    public CallMethod(Method target, int arity, bool splat, Register result, Loc loc)
    {
        Target = target;
        Arity = arity;
        Splat = splat;
        Result = result;
        Loc = loc;
    }

    public OpCode Code => OpCode.CallMethod;
    public string Dump(VM vm) => $"CallMethod {Target} {Arity} {Splat} {Result} {Loc}";
}