namespace Sharpl.Ops;

public readonly record struct PrepareClosure(UserMethod Target)
{
    public static Op Make(UserMethod Target)
    {
        return new Op(Op.T.PrepareClosure, new PrepareClosure(Target));
    }

    public override string ToString() {
        return $"PrepareClosure {Target}";
    }    
}