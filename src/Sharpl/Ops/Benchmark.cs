namespace Sharpl.Ops;

public class Benchmark : Op
{
    public static Op Make(int n) => new Benchmark(n);
    public readonly int N;

    public Benchmark(int n): base(OpCode.Benchmark)
    {
        N = n;
    }

    public override string Dump(VM vm) => $"Benchmark {N}";
}