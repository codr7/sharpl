namespace Sharpl.Ops;

public readonly record struct DoRead(Loc Loc, int FrameOffset, int Index)
{
    public static Op Make(Loc loc, int frameOffset, int index)
    {
        return new Op(Op.T.DoRead, new DoRead(loc, frameOffset, index));
    }

    public override string ToString() {
        return $"OpenStreamReader {Loc} {FrameOffset}:{Index}";
    }
}