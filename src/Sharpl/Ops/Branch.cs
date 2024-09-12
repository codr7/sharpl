namespace Sharpl.Ops;

public readonly record struct Branch(Loc Loc, Label Right) : Op
{
    public static Op Make(Loc loc, Label right) => new Branch(loc, right);
    public override string ToString() => $"Branch {Loc} {Right}";
}