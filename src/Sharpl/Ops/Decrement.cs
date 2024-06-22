namespace Sharpl.Ops;

public readonly record struct Decrement(int FrameOffset, int Index)
{
    public static Op Make(int frameOffset, int index)
    {
        return new Op(Op.T.Decrement, new Decrement(frameOffset, index));
    }

    public override string ToString() {
        return $"Decrement {FrameOffset}:{Index}";
    }
}