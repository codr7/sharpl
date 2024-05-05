namespace Sharpl.Ops;

public readonly record struct CallMethod(Loc Loc, Method Target, int Arity)
{
    public static Op Make(Loc loc, Method Target, int arity)
    {
        return new Op(Op.T.CallMethod, new CallMethod(loc, Target, arity));
    }

    public override string ToString() {
        return $"(call-method {Target} {Arity})";
    }    
}