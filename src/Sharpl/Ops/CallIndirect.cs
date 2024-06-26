namespace Sharpl.Ops;

public readonly record struct CallIndirect(Loc Loc, int Arity, bool Splat, int RegisterCount)
{
    public static Op Make(Loc loc, int arity, bool splat, int registerCount)
    {
        return new Op(Op.T.CallIndirect, new CallIndirect(loc, arity, splat, registerCount));
    }

    public override string ToString() {
        return $"CallIndirect {Loc} {Arity} {Splat} {RegisterCount}";
    }    
}