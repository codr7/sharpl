namespace Sharpl.Ops;

public readonly record struct Benchmark(int N) : Op
{
    public static Op Make(int n) => new Benchmark(n);
    public override string ToString() => $"Benchmark {N}";
}