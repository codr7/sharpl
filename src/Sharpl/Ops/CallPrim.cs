namespace Sharpl.Ops;

public readonly record struct CallPrim(Loc Loc, Method Target, int Arity)
{
    public static Op Make(Loc loc, Method Target, int arity)
    {
        return new Op(Op.T.CallPrim, new CallPrim(loc, Target, arity));
    }

    public override string ToString() {
        return $"(call-prim {Target} {Arity})";
    }
}