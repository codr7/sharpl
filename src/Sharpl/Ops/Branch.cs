namespace Sharpl.Ops;

public readonly record struct Branch(Label Right)
{
    public static Op Make(Label right) => new Op(Op.T.Branch, new Branch(right));
    public override string ToString() => $"Branch {Right}";
}