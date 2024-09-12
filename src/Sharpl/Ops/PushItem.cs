namespace Sharpl.Ops;

public readonly record struct PushItem(Loc Loc, Register Target) : Op
{
    public static Op Make(Loc loc, Register target) => new PushItem(loc, target);
    public override string ToString() => $"PushItem {Loc} {Target}";
}