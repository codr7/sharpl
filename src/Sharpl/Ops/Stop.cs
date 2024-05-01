namespace Sharpl.Ops;
public readonly record struct Stop()
{
    public static Op Instance = new Op(Op.T.Stop, new Stop());

    public static Op Make()
    {
        return Instance;
    }

    public override string ToString() {
        return $"(stop)";
    }
}