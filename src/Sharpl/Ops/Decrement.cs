namespace Sharpl.Ops;

public readonly record struct Decrement(Register Target, int Delta)
{
    public static Op Make(Register target, int delta) => new Op(Op.T.Decrement, new Decrement(target, delta));
    public override string ToString() => $"Decrement {Target} {Delta}";
}