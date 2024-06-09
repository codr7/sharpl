namespace Sharpl.Ops;

public readonly record struct SetRegister(int FrameOffset, int Index)
{
    public static Op Make(int frameOffset, int index)
    {
        return new Op(Op.T.SetRegister, new SetRegister(frameOffset, index));
    }

    public override string ToString() {
        return $"(set-register {FrameOffset} {Index})";
    }
}