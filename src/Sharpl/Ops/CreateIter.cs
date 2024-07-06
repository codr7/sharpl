namespace Sharpl.Ops;

public readonly record struct CreateIter(Loc Loc, Register Target)
{
    public static Op Make(Loc loc, Register target)
    {
        return new Op(Op.T.CreateIter, new CreateIter(loc, target));
    }

    public override string ToString() {
        return $"CreateIter {Loc} {Target}";
    }    
}