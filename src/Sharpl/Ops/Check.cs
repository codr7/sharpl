namespace Sharpl.Ops;

public readonly record struct Check(Loc Loc)
{
    public static Op Make(Loc loc)
    {
        return new Op(Op.T.Check, new Check(loc));
    }

    public override string ToString() {
        return $"Check {Loc}";
    }
}