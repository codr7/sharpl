namespace Sharpl.Ops;

public readonly record struct CallUserMethod(Loc Loc, UserMethod Target, int Arity, int RegisterCount)
{
    public static Op Make(Loc loc, UserMethod Target, int arity, int registerCount)
    {
        return new Op(Op.T.CallUserMethod, new CallUserMethod(loc, Target, arity, registerCount));
    }

    public override string ToString() {
        return $"CallUserMethod {Target} {Arity} {RegisterCount}";
    }    
}