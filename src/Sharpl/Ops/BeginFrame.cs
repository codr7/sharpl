namespace Sharpl.Ops;

public readonly record struct BeginFrame()
{
    public static Op Make()
    {
        return new Op(Op.T.BeginFrame, new BeginFrame());
    }

    public override string ToString() {
        return "(begin-frame)";
    }    
}