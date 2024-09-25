namespace Sharpl.Ops;

public class Benchmark : Op
{
    public static Op Make(int reps) => new Benchmark(reps);
    public readonly int Reps;

    public Benchmark(int reps)
    {
        Reps = reps;
    }

    public OpCode Code => OpCode.Benchmark;
    public string Dump(VM vm) => $"Benchmark {Reps}";
}