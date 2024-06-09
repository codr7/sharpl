namespace Sharpl.Ops;

public readonly record struct CallIndirect(Loc Loc, int Arity)
{
    public static Op Make(Loc loc, int arity)
    {
        return new Op(Op.T.CallIndirect, new CallIndirect(loc, arity));
    }

    public override string ToString() {
        return $"CallIndirect {Loc} {Arity}";
    }    
}