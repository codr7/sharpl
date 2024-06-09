namespace Sharpl.Ops;

public readonly record struct Check(Loc Loc, Form Expected)
{
    public static Op Make(Loc loc, Form expected)
    {
        return new Op(Op.T.Check, new Check(loc, expected));
    }

    public override string ToString() {
        return $"Check {Loc} {Expected}";
    }
}