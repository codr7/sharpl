namespace Sharpl.Ops;

public readonly record struct GetRegister(Register Target) : Op
{
    public static Op Make(Register target) => new GetRegister(target);
    public override string ToString() => $"GetRegister {Target}";
}