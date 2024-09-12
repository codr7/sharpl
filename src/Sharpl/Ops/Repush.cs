namespace Sharpl.Ops;
public readonly record struct Repush(int N) : Op
{
    public static Op Make(int n) => new Repush(n);
    public override string ToString() => $"Repush {N}";
}