namespace Sharpl.Ops;

public readonly record struct CallStack(Loc Loc, int Arity, bool Splat, int RegisterCount) : Op
{
    public static Op Make(Loc loc, int arity, bool splat, int registerCount) =>
        new CallStack(loc, arity, splat, registerCount);

    public override string ToString() =>
        $"CallStack {Loc} {Arity} {Splat} {RegisterCount}";
}