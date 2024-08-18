namespace Sharpl.Ops;

public readonly record struct CallMethod(Loc Loc, Method Target, int Arity, bool Splat)
{
    public static Op Make(Loc loc, Method target, int arity, bool splat) => 
        new Op(Op.T.CallMethod, new CallMethod(loc, target, arity, splat));

    public override string ToString() => $"CallMethod {Target} {Arity} {Splat}";
}