namespace Sharpl.Ops;

public class BeginFrame : Op
{
    public static Op Make(int registerCount) => new BeginFrame(registerCount);
    public readonly int RegisterCount;

    public BeginFrame(int registerCount): base(OpCode.BeginFrame)
    {
        RegisterCount = registerCount;
    }

    public override string Dump(VM vm) => $"BeginFrame {RegisterCount}";
}