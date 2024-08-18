namespace Sharpl.Ops;

public readonly record struct Drop(int Count)
{
    public static Op Make(int count) => new Op(Op.T.Drop, new Drop(count));
    public override string ToString() => $"Drop {Count}";
}