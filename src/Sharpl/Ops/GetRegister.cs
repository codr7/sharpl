namespace Sharpl.Ops;

public readonly record struct GetRegister(Register Target)
{
    public static Op Make(Register target) => new Op(Op.T.GetRegister, new GetRegister(target));
    public override string ToString() => $"GetRegister {Target}";
}