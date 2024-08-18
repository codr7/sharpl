namespace Sharpl.Ops;

public readonly record struct Check(Loc Loc)
{
    public static Op Make(Loc loc) => new Op(Op.T.Check, new Check(loc));
    public override string ToString() => $"Check {Loc}";
}