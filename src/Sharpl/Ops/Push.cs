namespace Sharpl.Ops;

public readonly record struct Push(Value Value) : Op
{
    public static Op Make(Value value) => new Push(value);
    public override string ToString() => $"Push {Value}";
}