namespace Sharpl.Ops;

public class CallMethod : Op
{
    public static Op Make(Method target, int arity, bool splat, Loc loc) =>
        new CallMethod(target, arity, splat, loc);

    public readonly Loc Loc;
    public readonly Method Target;
    public readonly int Arity;
    public readonly bool Splat;

    public CallMethod(Method target, int arity, bool splat, Loc loc): base(OpCode.CallMethod)
    {
        Loc = loc;
        Target = target;
        Arity = arity;
        Splat = splat;
    }

    public override string Dump(VM vm) => $"CallMethod {Target} {Arity} {Splat}";
}