namespace Sharpl.Ops;

public readonly record struct OpenInputStream(Loc Loc, int FrameOffset, int Index) : Op
{
    public static Op Make(Loc loc, int frameOffset, int index) =>
        new OpenInputStream(loc, frameOffset, index);

    public override string ToString() => $"OpenInputStream {Loc} {FrameOffset}:{Index}";
}