namespace Sharpl.Ops;
public readonly record struct ExitMethod() : Op
{
    public static Op Instance = new ExitMethod();
    public static Op Make() => Instance;
    public override string ToString() => $"ExitMethod";
}