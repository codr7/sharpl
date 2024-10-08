namespace Sharpl.Ops;

public class CallStack : Op
{
    public static Op Make(int arity, bool splat, int registerCount, Loc loc) =>
        new CallStack(arity, splat, registerCount, loc);

    public readonly int Arity;
    public readonly bool Splat;
    public readonly int RegisterCount;
    public readonly Loc Loc;

    public CallStack(int arity, bool splat, int registerCount, Loc loc)
    {
        Arity = arity;
        Splat = splat;
        RegisterCount = registerCount;
        Loc = loc;
    }

    public OpCode Code => OpCode.CallStack;
    public string Dump(VM vm) => $"CallStack {Loc} {Arity} {Splat} {RegisterCount}";
}