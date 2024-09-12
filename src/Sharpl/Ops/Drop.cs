namespace Sharpl.Ops;

public readonly record struct Drop(int Count) : Op
{
    public static Op Make(int count) => new Drop(count);
    public override string ToString() => $"Drop {Count}";
}