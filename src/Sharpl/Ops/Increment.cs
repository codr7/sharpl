namespace Sharpl.Ops;

public readonly record struct Increment(Register Target, int Delta) : Op
{
    public static Op Make(Register target, int delta) => new Increment(target, delta);
    public override string ToString() => $"Increment {Target} {Delta}";
}