namespace Sharpl.Ops;
public readonly record struct PushSplat()
{
    public static Op Instance = new Op(Op.T.PushSplat, new PushSplat());

    public static Op Make()
    {
        return Instance;
    }

    public override string ToString() {
        return $"PushSplat";
    }
}