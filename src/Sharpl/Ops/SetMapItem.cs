namespace Sharpl.Ops;

public readonly record struct SetMapItem()
{
    public static Op Make() => new Op(Op.T.SetMapItem, new SetMapItem());
    public override string ToString() => $"SetMapItem";
}