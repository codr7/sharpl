namespace Sharpl.Ops;

public readonly record struct PrepareClosure(UserMethod Target, Label Skip) : Op
{
    public static Op Make(UserMethod target, Label skip) => new PrepareClosure(target, skip);
    public override string ToString() => $"PrepareClosure {Target} {Skip}";
}