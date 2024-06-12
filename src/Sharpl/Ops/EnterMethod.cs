namespace Sharpl.Ops;

public readonly record struct EnterMethod(UserMethod Target)
{
    public static Op Make(UserMethod target)
    {
        return new Op(Op.T.EnterMethod, new EnterMethod(target));
    }

    public override string ToString() {
        return $"EnterMethod {Target.Name}";
    }    
}