namespace Sharpl.Ops;

public readonly record struct PopItem(Loc Loc, Register Target)
{
    public static Op Make(Loc loc, Register target) => new Op(Op.T.PopItem, new PopItem(loc, target));
    public override string ToString() => $"PopItem {Loc} {Target}";
}