namespace Sharpl.Ops;

public readonly record struct CreateList(Register Target)
{
    public static Op Make(Register target) => new Op(Op.T.CreateList, new CreateList(target));
    public override string ToString() => $"CreateList {Target}";
}