namespace Sharpl.Ops;

public readonly record struct BeginFrame(int RegisterCount) : Op
{
    public static Op Make(int registerCount) => new BeginFrame(registerCount);
    public override string ToString() => $"BeginFrame {RegisterCount}";
}