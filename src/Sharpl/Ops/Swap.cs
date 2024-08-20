namespace Sharpl.Ops;

public readonly record struct Swap(Loc Loc)
{
    public static Op Make(Loc loc) => new Op(Op.T.Swap, new Swap(loc));
    public override string ToString() => $"Swap {Loc}";
}