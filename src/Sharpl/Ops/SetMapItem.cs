namespace Sharpl.Ops;

public readonly record struct SetMapItem() : Op
{
    public static Op Make() => new SetMapItem();
    public override string ToString() => $"SetMapItem";
}