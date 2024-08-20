namespace Sharpl.Ops;

public readonly record struct Unzip(Loc Loc)
{
    public static Op Make(Loc loc) => new Op(Op.T.Unzip, new Unzip(loc));
    public override string ToString() => $"Unzip {Loc}";
}