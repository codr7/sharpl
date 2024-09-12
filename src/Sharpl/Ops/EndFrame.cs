namespace Sharpl.Ops;

public readonly record struct EndFrame() : Op
{
    public static Op Make() => new EndFrame();
    public override string ToString() => "EndFrame";
}