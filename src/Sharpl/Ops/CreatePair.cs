namespace Sharpl.Ops;

public readonly record struct CreatePair(Loc Loc) : Op
{
    public static Op Make(Loc loc) => new CreatePair(loc);
    public override string ToString() => $"CreatePair {Loc}";
}