namespace Sharpl.Ops;

public readonly record struct OpenInputStream(Loc Loc, int FrameOffset, int Index)
{
    public static Op Make(Loc loc, int frameOffset, int index) =>
        new Op(Op.T.OpenInputStream, new OpenInputStream(loc, frameOffset, index));

    public override string ToString() => $"OpenInputStream {Loc} {FrameOffset}:{Index}";
}