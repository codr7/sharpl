namespace Sharpl.Ops;
public readonly record struct ExitMethod()
{
    public static Op Instance = new Op(Op.T.ExitMethod, new ExitMethod());

    public static Op Make()
    {
        return Instance;
    }

    public override string ToString() {
        return $"ExitMethod";
    }
}