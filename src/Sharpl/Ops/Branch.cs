namespace Sharpl.Ops;

public readonly record struct Branch(Label Right)
{
    public static Op Make(Label right)
    {
        return new Op(Op.T.Branch, new Branch(right));
    }

    public override string ToString() {
        return $"Branch {Right}";
    }
}