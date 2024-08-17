namespace Sharpl.Ops;

public readonly record struct Decrement(Register Target)
{
    public static Op Make(Register target) => new Op(Op.T.Decrement, new Decrement(target));
    public override string ToString() => $"Decrement {Target}";
}