namespace Sharpl.Ops;

public readonly record struct IterNext(Loc Loc, Register Iter, Label Done, bool Push) : Op
{
    public static Op Make(Loc loc, Register iter, Label done, bool push = true) =>
        new IterNext(loc, iter, done, push);

    public override string ToString() => $"IterNext {Loc} {Iter} {Done} {Push}";
}