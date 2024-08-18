namespace Sharpl.Ops;

public readonly record struct CopyRegister(int FromFrameOffset, int FromIndex, int ToFrameOffset, int ToIndex)
{
    public static Op Make(int fromFrameOffset, int fromIndex, int toFrameOffset, int toIndex) =>
        new Op(Op.T.CopyRegister, new CopyRegister(fromFrameOffset, fromIndex, toFrameOffset, toIndex));

    public override string ToString() =>
        $"CopyRegister {FromFrameOffset}:{FromIndex} {ToFrameOffset}:{ToIndex}";
}