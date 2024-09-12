namespace Sharpl.Ops;

public readonly record struct Goto(Label Target) : Op
{
    public static Op Make(Label target) => new Goto(target);
    public override string ToString() => $"Goto {Target}";
}