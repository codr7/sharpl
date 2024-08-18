namespace Sharpl.Ops;

public readonly record struct CreatePair(Loc Loc)
{
    public static Op Make(Loc loc) => new Op(Op.T.CreatePair, new CreatePair(loc));
    public override string ToString() => $"CreatePair {Loc}";
}