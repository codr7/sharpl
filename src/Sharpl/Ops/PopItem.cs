namespace Sharpl.Ops;

public readonly record struct PopItem(Loc Loc, Register Target) : Op
{
    public static Op Make(Loc loc, Register target) => new PopItem(loc, target);
    public override string ToString() => $"PopItem {Loc} {Target}";
}