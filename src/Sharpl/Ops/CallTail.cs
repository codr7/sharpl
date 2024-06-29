namespace Sharpl.Ops;

public readonly record struct CallTail(Loc Loc, UserMethod Target, int Arity, bool Splat)
{
    public static Op Make(Loc loc, UserMethod target, int arity, bool splat)
    {
        return new Op(Op.T.CallTail, new CallTail(loc, target, arity, splat));
    }

    public override string ToString() {
        return $"BindArgs {Loc} {Target} {Arity} {Splat}";
    }    
}