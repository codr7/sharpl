namespace Sharpl.Ops;

public readonly record struct CallIndirect(Loc Loc, int Arity, int RegisterCount)
{
    public static Op Make(Loc loc, int arity, int registerCount)
    {
        return new Op(Op.T.CallIndirect, new CallIndirect(loc, arity, registerCount));
    }

    public override string ToString() {
        return $"CallIndirect {Loc} {Arity} {RegisterCount}";
    }    
}