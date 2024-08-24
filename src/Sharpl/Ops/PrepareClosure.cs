namespace Sharpl.Ops;

public readonly record struct PrepareClosure(UserMethod Target, Label Skip)
{
    public static Op Make(UserMethod target, Label skip) => new Op(Op.T.PrepareClosure, new PrepareClosure(target, skip));
    public override string ToString() => $"PrepareClosure {Target} {Skip}";
}