namespace Sharpl.Ops;

public class CallRegister : Op
{
    public static Op Make(Register target, int arity, bool splat, int registerCount, Register result, Loc loc) =>
        new CallRegister(target, arity, splat, registerCount, result, loc);

    public readonly Loc Loc;
    public readonly Register Target;
    public readonly int Arity;
    public readonly bool Splat;
    public readonly int RegisterCount;
    public readonly Register Result;

    public CallRegister(Register target, int arity, bool splat, int registerCount, Register result, Loc loc)
    {
        Target = target;
        Arity = arity; 
        Splat = splat;
        RegisterCount = registerCount;
        Result = result;
        Loc = loc;
    }

    public OpCode Code => OpCode.CallRegister;
    public string Dump(VM vm) => $"CallRegister {Loc} {Target} {Arity} {Splat} {RegisterCount} {Result} {Loc}";
}