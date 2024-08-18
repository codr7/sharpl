namespace Sharpl.Ops;

public readonly record struct Push(Value Value)
{
    public static Op Make(Value value) => new Op(Op.T.Push, new Push(value));
    public override string ToString() => $"Push {Value}";
}