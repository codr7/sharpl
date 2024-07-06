namespace Sharpl.Ops;

public readonly record struct IterNext(Loc Loc, Register Iter, Label Done)
{
    public static Op Make(Loc loc, Register iter, Label done)
    {
        return new Op(Op.T.IterNext, new IterNext(loc, iter, done));
    }

    public override string ToString() {
        return $"IterNext {Loc} {Iter} {Done}";
    }    
}