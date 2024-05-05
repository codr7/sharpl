namespace Sharpl.Ops;

public readonly record struct CallDirect(Loc Loc, Value Target, int Arity)
{
    public static Op Make(Loc loc, Value Target, int arity)
    {
        return new Op(Op.T.CallDirect, new CallDirect(loc, Target, arity));
    }

    public override string ToString() {
        return $"(call-direct {Arity})";
    }    
}