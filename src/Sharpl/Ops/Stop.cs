namespace Sharpl.Ops;
public readonly record struct Stop()
{
    public static readonly Op Instance = new Op(Op.T.Stop, new Stop());
    public static Op Make() => Instance;
    public override string ToString() => $"Stop";
}