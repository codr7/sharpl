namespace Sharpl.Ops;

public readonly record struct CallDirect(Loc Loc, Value Target, int Arity, int RegisterCount)
{
    public static Op Make(Loc loc, Value Target, int arity, int registerCount)
    {
        return new Op(Op.T.CallDirect, new CallDirect(loc, Target, arity, registerCount));
    }

    public override string ToString() {
        return $"CallDirect {Loc} {Target} {Arity} {RegisterCount}";
    }    
}