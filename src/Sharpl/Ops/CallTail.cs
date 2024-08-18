namespace Sharpl.Ops;

public readonly record struct CallTail(Loc Loc, UserMethod Target, Value?[] ArgMask, bool Splat)
{
    public static Op Make(Loc loc, UserMethod target, Value?[] argMask, bool splat) =>
        new Op(Op.T.CallTail, new CallTail(loc, target, argMask, splat));

    public override string ToString() => $"CallTail {Loc} {Target} {ArgMask} {Splat}";
}