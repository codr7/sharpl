namespace Sharpl.Ops;

public readonly record struct Splat(Loc Loc) : Op
{
    public static Op Make(Loc loc) => new Splat(loc);
    public override string ToString() => $"Splat {Loc}";
}