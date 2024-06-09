namespace Sharpl.Ops;

public readonly record struct Goto(Label Target)
{
    public static Op Make(Label target)
    {
        return new Op(Op.T.Goto, new Goto(target));
    }

    public override string ToString() {
        return $"Goto {Target}";
    }
}