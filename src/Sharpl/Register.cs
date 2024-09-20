namespace Sharpl;

public readonly record struct Register(int FrameOffset, int Index)
{
    public override string ToString() => $"(Register {FrameOffset} {Index})";
}