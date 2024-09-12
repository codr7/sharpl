namespace Sharpl.Ops;

public readonly record struct SetRegister(Register Target) : Op
{
    public static Op Make(Register target) => new SetRegister(target);
    public override string ToString() => $"SetRegister {Target}";
}