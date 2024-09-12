namespace Sharpl.Ops;

public readonly record struct CreateIter(Loc Loc, Register Target) : Op
{
    public static Op Make(Loc loc, Register target) => new CreateIter(loc, target);
    public override string ToString() => $"CreateIter {Loc} {Target}";
}