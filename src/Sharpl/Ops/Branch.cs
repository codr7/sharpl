namespace Sharpl.Ops;

public readonly record struct Branch(Loc Loc, Label Right)
{
    public static Op Make(Loc loc, Label right) => new Op(Op.T.Branch, new Branch(loc, right));
    public override string ToString() => $"Branch {Loc} {Right}";
}