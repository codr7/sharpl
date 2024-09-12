namespace Sharpl.Ops;
public readonly record struct Stop() : Op
{
    public static readonly Op Instance = new Stop();
    public static Op Make() => Instance;
    public override string ToString() => $"Stop";
}