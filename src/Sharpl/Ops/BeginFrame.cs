namespace Sharpl.Ops;

public class BeginFrame : Op
{
    public static Op Make(int registerCount) => new BeginFrame(registerCount);
    public readonly int RegisterCount;

    public BeginFrame(int registerCount)
    {
        RegisterCount = registerCount;
    }

    public OpCode Code => OpCode.BeginFrame;
    public string Dump(VM vm) => $"BeginFrame {RegisterCount}";
}