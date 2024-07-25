namespace Sharpl.Ops;
public readonly record struct PushSplat()
{
    public static Op Make()
    {
        return new Op(Op.T.PushSplat, new PushSplat());
    }

    public override string ToString() {
        return "PushSplat";
    }
}