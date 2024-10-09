namespace Sharpl.Ops;

public class Benchmark : Op
{
    public static Op Make(int reps, Register result) => new Benchmark(reps, result);
    
    public readonly int Reps;
    public readonly Register Result;

    public Benchmark(int reps, Register result)
    {
        Reps = reps;
        Result = result;
    }

    public OpCode Code => OpCode.Benchmark;
    public string Dump(VM vm) => $"Benchmark {Reps} {Result}";
}