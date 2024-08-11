namespace Sharpl.Ops;

public readonly record struct QuoteCall(Loc Loc, int Arity)
{
    public static Op Make(Loc loc, int arity)
    {
        return new Op(Op.T.QuoteCall, new QuoteCall(loc, arity));
    }

    public override string ToString() {
        return $"QuoteCall {Loc} {Arity}";
    }
}