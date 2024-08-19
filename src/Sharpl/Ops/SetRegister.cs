namespace Sharpl.Ops;

public readonly record struct SetRegister(Register Target)
{
    public static Op Make(Register target) =>
        new Op(Op.T.SetRegister, new SetRegister(target));

    public override string ToString() => $"SetRegister {Target}";
}