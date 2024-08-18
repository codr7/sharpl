namespace Sharpl.Ops;

public readonly record struct Splat(Loc Loc)
{
    public static Op Make(Loc loc) => new Op(Op.T.Splat, new Splat(loc));
    public override string ToString() => $"Splat {Loc}";
}