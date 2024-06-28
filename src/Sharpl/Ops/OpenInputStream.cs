namespace Sharpl.Ops;

public readonly record struct OpenInputStream(Loc Loc, int FrameOffset, int Index)
{
    public static Op Make(Loc loc, int frameOffset, int index)
    {
        return new Op(Op.T.OpenInputStream, new OpenInputStream(loc, frameOffset, index));
    }

    public override string ToString() {
        return $"OpenInputStream {Loc} {FrameOffset}:{Index}";
    }
}