namespace Sharpl.Ops;

public readonly record struct Branch(Label Target)
{
    public static Op Make(Label target)
    {
        return new Op(Op.T.Branch, new Branch(target));
    }

    public override string ToString() {
        return $"Branch {Target}";
    }
}