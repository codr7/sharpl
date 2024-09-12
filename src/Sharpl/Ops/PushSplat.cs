namespace Sharpl.Ops;
public readonly record struct PushSplat() : Op
{
    public static Op Make() => new PushSplat();
    public override string ToString() => "PushSplat";
}