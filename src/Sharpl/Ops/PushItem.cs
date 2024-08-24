namespace Sharpl.Ops;

public readonly record struct PushItem(Loc Loc, Register Target)
{
    public static Op Make(Loc loc, Register target) => new Op(Op.T.PushItem, new PushItem(loc, target));
    public override string ToString() => $"PushItem {Loc} {Target}";
}