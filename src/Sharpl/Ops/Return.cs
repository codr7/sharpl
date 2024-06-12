namespace Sharpl.Ops;
public readonly record struct Return()
{
    public static Op Instance = new Op(Op.T.Return, new Return());

    public static Op Make()
    {
        return Instance;
    }

    public override string ToString() {
        return $"Return";
    }
}