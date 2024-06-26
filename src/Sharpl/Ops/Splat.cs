namespace Sharpl.Ops;

public readonly record struct Splat(Loc Loc)
{
    public static Op Make(Loc loc)
    {
        return new Op(Op.T.Splat, new Splat(loc));
    }

    public override string ToString() {
        return $"Splat {Loc}";
    }
}