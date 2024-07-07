namespace Sharpl.Ops;

public readonly record struct Drop(int Count)
{
    public static Op Make(int count)
    {
        return new Op(Op.T.Drop, new Drop(count));
    }

    public override string ToString() {
        return $"Drop {Count}";
    }
}