namespace Sharpl.Ops;

public readonly record struct CallRegister(Loc Loc, int TargetFrameOffset, int TargetIndex, int Arity, bool Splat, int RegisterCount)
{
    public static Op Make(Loc loc, int targetFrameOffset, int targetIndex, int arity, bool splat, int registerCount)
    {
        return new Op(Op.T.CallRegister, new CallRegister(loc, targetFrameOffset, targetIndex, arity, splat, registerCount));
    }

    public override string ToString() {
        return $"CallRegister {Loc} {TargetFrameOffset}:{TargetIndex} {Arity} {Splat} {RegisterCount}";
    }    
}