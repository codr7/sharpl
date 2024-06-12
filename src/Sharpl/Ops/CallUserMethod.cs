namespace Sharpl.Ops;

public readonly record struct CallUserMethod(Loc Loc, UserMethod Target, int Arity)
{
    public static Op Make(Loc loc, UserMethod Target, int arity)
    {
        return new Op(Op.T.CallUserMethod, new CallUserMethod(loc, Target, arity));
    }

    public override string ToString() {
        return $"CallUserMethod {Target.Name} {Arity}";
    }    
}