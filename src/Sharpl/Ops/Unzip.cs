namespace Sharpl.Ops;

public readonly record struct Unzip(Loc Loc) : Op
{
    public static Op Make(Loc loc) => new Unzip(loc);
    public override string ToString() => $"Unzip {Loc}";
}