namespace Sharpl.Ops;

public readonly record struct EndFrame()
{
    public static Op Make()
    {
        return new Op(Op.T.EndFrame, new EndFrame());
    }

    public override string ToString() {
        return "EndFrame";
    }    
}