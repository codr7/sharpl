namespace Sharpl.Ops;

public readonly record struct CallUserMethod(Loc Loc, UserMethod Target, int Arity, bool Splat, int RegisterCount)
{
    public static Op Make(Loc loc, UserMethod Target, int arity, bool splat, int registerCount)
    {
        return new Op(Op.T.CallUserMethod, new CallUserMethod(loc, Target, arity, splat, registerCount));
    }

    public override string ToString() {
        return $"CallUserMethod {Target} {Arity} {Splat} {RegisterCount}";
    }    
}