namespace Sharpl.Ops;

public readonly record struct Swap(Loc Loc) : Op
{
    public static Op Make(Loc loc) => new Swap(loc);
    public override string ToString() => $"Swap {Loc}";
}