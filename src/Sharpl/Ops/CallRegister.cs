namespace Sharpl.Ops;

public readonly record struct CallRegister(Loc Loc, Register Target, int Arity, bool Splat, int RegisterCount) : Op
{
    public static Op Make(Loc loc, Register target, int arity, bool splat, int registerCount) =>
        new CallRegister(loc, target, arity, splat, registerCount);

    public override string ToString() =>
        $"CallRegister {Loc} {Target} {Arity} {Splat} {RegisterCount}";
}