namespace Sharpl.Ops;

public readonly record struct GetRegister(int FrameOffset, int Index)
{
    public static Op Make(int frameOffset, int index)
    {
        return new Op(Op.T.GetRegister, new GetRegister(frameOffset, index));
    }

    public override string ToString() {
        return $"GetRegister {FrameOffset} {Index}";
    }
}