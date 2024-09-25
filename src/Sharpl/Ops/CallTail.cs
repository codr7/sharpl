namespace Sharpl.Ops;

public class CallTail : Op
{
    public static Op Make(UserMethod target, Value?[] argMask, bool splat, Loc loc) =>
        new CallTail(target, argMask, splat, loc);

    public readonly UserMethod Target;
    public readonly Value?[] ArgMask;
    public readonly bool Splat;
    public readonly Loc Loc;

    public CallTail(UserMethod target, Value?[] argMask, bool splat, Loc loc)
    {
        Target = target;
        ArgMask = argMask; 
        Splat = splat; 
        Loc = loc;
    }
    public OpCode Code => OpCode.CallTail;
    public string Dump(VM vm) => $"CallTail {Loc} {Target} {ArgMask} {Splat}";
}