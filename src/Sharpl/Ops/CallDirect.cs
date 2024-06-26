namespace Sharpl.Ops;

public readonly record struct CallDirect(Loc Loc, Value Target, int Arity, bool Splat, int RegisterCount)
{
    public static Op Make(Loc loc, Value Target, int arity, bool splat, int registerCount)
    {
        return new Op(Op.T.CallDirect, new CallDirect(loc, Target, arity, splat, registerCount));
    }

    public override string ToString() {
        return $"CallDirect {Loc} {Target} {Arity} {Splat} {RegisterCount}";
    }    
}