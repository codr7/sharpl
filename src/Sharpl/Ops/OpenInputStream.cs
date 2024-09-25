namespace Sharpl.Ops;

public class OpenInputStream : Op
{
    public static Op Make(int frameOffset, int index, Loc loc) =>
        new OpenInputStream(frameOffset, index, loc);

    public readonly int FrameOffset;
    public readonly int Index;
    public readonly Loc Loc;

    public OpenInputStream(int frameOffset, int index, Loc loc): base(OpCode.OpenInputStream)
    {
        FrameOffset = frameOffset;
        Index = index;
        Loc = loc;
    }

    public override string Dump(VM vm) => $"OpenInputStream {Loc} {FrameOffset}:{Index}";
}