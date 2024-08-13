namespace Sharpl.Ops;

public readonly record struct CallUserMethod(Loc Loc, UserMethod Target, Value?[] ArgMask, bool Splat, int RegisterCount)
{
    public static Op Make(Loc loc, UserMethod Target, Value?[] argMask, bool splat, int registerCount)
    {
        return new Op(Op.T.CallUserMethod, new CallUserMethod(loc, Target, argMask, splat, registerCount));
    }

    public override string ToString() {
        return $"CallUserMethod {Target} {ArgMask} {Splat} {RegisterCount}";
    }    
}