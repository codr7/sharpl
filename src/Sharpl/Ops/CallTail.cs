namespace Sharpl.Ops;

public class CallTail : Op
{
    public static Op Make(UserMethod target, Value?[] argMask, bool splat, Register result, Loc loc) =>
        new CallTail(target, argMask, splat, result, loc);

    public readonly UserMethod Target;
    public readonly Value?[] ArgMask;
    public readonly bool Splat;
    public readonly Register Result;
    public readonly Loc Loc;

    public CallTail(UserMethod target, Value?[] argMask, bool splat, Register result, Loc loc)
    {
        Target = target;
        ArgMask = argMask; 
        Splat = splat;
        Result = result;
        Loc = loc;
    }
    public OpCode Code => OpCode.CallTail;
    public string Dump(VM vm) => $"CallTail {Target} {ArgMask} {Splat} {Result} {Loc}";
}