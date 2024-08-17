namespace Sharpl.Ops;

public readonly record struct Increment(Register Target)
{
    public static Op Make(Register target) => new Op(Op.T.Increment, new Increment(target));
    public override string ToString() => $"Increment {Target}";
}