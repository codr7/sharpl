namespace Sharpl.Ops;

public readonly record struct CallUserMethod(Loc Loc, UserMethod Target, Value?[] ArgMask, bool Splat, int RegisterCount) : Op
{
    public static Op Make(Loc loc, UserMethod Target, Value?[] argMask, bool splat, int registerCount) =>
        new CallUserMethod(loc, Target, argMask, splat, registerCount);

    public override string ToString() =>
        $"CallUserMethod {Target} {ArgMask} {Splat} {RegisterCount}";
}