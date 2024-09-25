namespace Sharpl.Ops;

public class CallUserMethod : Op
{
    public static Op Make(UserMethod Target, Value?[] argMask, bool splat, int registerCount, Loc loc) =>
        new CallUserMethod(Target, argMask, splat, registerCount, loc);

    public readonly UserMethod Target;
    public readonly Value?[] ArgMask;
    public readonly bool Splat;
    public readonly int RegisterCount;
    public readonly Loc Loc;

    public CallUserMethod(UserMethod target, Value?[] argMask, bool splat, int registerCount, Loc loc): base(OpCode.CallUserMethod)
    {
        Target = target;
        ArgMask = argMask;
        Splat = splat;
        RegisterCount = registerCount;
        Loc = loc;
    }

    public override string Dump(VM vm) =>
        $"CallUserMethod {Target} {ArgMask} {Splat} {RegisterCount}";
}