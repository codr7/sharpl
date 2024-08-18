namespace Sharpl.Ops;

public readonly record struct PrepareClosure(UserMethod Target)
{
    public static Op Make(UserMethod Target) => new Op(Op.T.PrepareClosure, new PrepareClosure(Target));
    public override string ToString() => $"PrepareClosure {Target}";
}