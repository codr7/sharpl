namespace Sharpl.Ops;

public readonly record struct CreateArray(int Length)
{
    public static Op Make(int length) => new Op(Op.T.CreateArray, new CreateArray(length));
    public override string ToString() => $"CreateArray {Length}";
}