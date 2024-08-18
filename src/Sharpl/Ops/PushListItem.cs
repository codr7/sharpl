namespace Sharpl.Ops;

public readonly record struct PushListItem(Loc Loc, Register Target)
{
    public static Op Make(Loc loc, Register target) => new Op(Op.T.PushListItem, new PushListItem(loc, target));
    public override string ToString() => $"PushListItem {Loc} {Target}";
}