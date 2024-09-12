namespace Sharpl.Ops;

public readonly record struct Decrement(Register Target, int Delta) : Op
{
    public static Op Make(Register target, int delta) => new Decrement(target, delta);
    public override string ToString() => $"Decrement {Target} {Delta}";
}