namespace Sharpl.Ops;

public readonly record struct Goto(Label Target)
{
    public static Op Make(Label target) => new Op(Op.T.Goto, new Goto(target));
    public override string ToString() => $"Goto {Target}";
}