namespace Sharpl.Ops;

public readonly record struct Benchmark(int N)
{
    public static Op Make(int n) => new Op(Op.T.Benchmark, new Benchmark(n));
    public override string ToString() => $"Benchmark {N}";
}