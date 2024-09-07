namespace Sharpl.Ops;

public readonly record struct Increment(Register Target, int Delta)
{
    public static Op Make(Register target, int delta) => new Op(Op.T.Increment, new Increment(target, delta));
    public override string ToString() => $"Increment {Target} {Delta}";
}