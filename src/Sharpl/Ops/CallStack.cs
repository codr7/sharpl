namespace Sharpl.Ops;

public readonly record struct CallStack(Loc Loc, int Arity, bool Splat, int RegisterCount)
{
    public static Op Make(Loc loc, int arity, bool splat, int registerCount) =>
        new Op(Op.T.CallStack, new CallStack(loc, arity, splat, registerCount));

    public override string ToString() =>
        $"CallStack {Loc} {Arity} {Splat} {RegisterCount}"; 
}