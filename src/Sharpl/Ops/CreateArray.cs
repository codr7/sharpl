namespace Sharpl.Ops;

public readonly record struct CreateArray(int Length) : Op
{
    public static Op Make(int length) => new CreateArray(length);
    public override string ToString() => $"CreateArray {Length}";
}