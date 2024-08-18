namespace Sharpl.Ops;

public readonly record struct CreateIter(Loc Loc, Register Target)
{
    public static Op Make(Loc loc, Register target) => new Op(Op.T.CreateIter, new CreateIter(loc, target));
    public override string ToString() => $"CreateIter {Loc} {Target}";
}