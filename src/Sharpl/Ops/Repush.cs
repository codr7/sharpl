namespace Sharpl.Ops;
public readonly record struct Repush(int N)
{
    public static Op Make(int n) =>
    new Op(Op.T.Repush, new Repush(n));
    public override string ToString() => $"Repush {N}";
}