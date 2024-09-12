namespace Sharpl.Ops;

public readonly record struct Check(Loc Loc) : Op
{
    public static Op Make(Loc loc) => new Check(loc);
    public override string ToString() => $"Check {Loc}";
}