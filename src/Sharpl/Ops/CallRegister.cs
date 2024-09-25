namespace Sharpl.Ops;

public class CallRegister : Op
{
    public static Op Make(Register target, int arity, bool splat, int registerCount, Loc loc) =>
        new CallRegister(target, arity, splat, registerCount, loc);

    public readonly Loc Loc;
    public readonly Register Target;
    public readonly int Arity;
    public readonly bool Splat;
    public readonly int RegisterCount;

    public CallRegister(Register target, int arity, bool splat, int registerCount, Loc loc) : base(OpCode.CallRegister)
    {
        Target = target;
        Arity = arity; 
        Splat = splat;
        RegisterCount = registerCount; 
        Loc = loc;
    }

    public override string Dump(VM vm) =>
        $"CallRegister {Loc} {Target} {Arity} {Splat} {RegisterCount}";
}