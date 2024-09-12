namespace Sharpl.Ops;

public readonly record struct CreateList(Register Target) : Op
{
    public static Op Make(Register target) => new CreateList(target);
    public override string ToString() => $"CreateList {Target}";
}