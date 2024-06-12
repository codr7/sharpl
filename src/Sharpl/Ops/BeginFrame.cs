namespace Sharpl.Ops;

public readonly record struct BeginFrame(int RegisterCount)
{
    public static Op Make(int registerCount)
    {
        return new Op(Op.T.BeginFrame, new BeginFrame(registerCount));
    }

    public override string ToString() {
        return $"BeginFrame {RegisterCount}";
    }    
}