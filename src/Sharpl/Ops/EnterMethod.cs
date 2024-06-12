namespace Sharpl.Ops;

public readonly record struct EnterMethod(UserMethod Target, int RegisterCount)
{
    public static Op Make(UserMethod target, int registerCount)
    {
        return new Op(Op.T.EnterMethod, new EnterMethod(target, registerCount));
    }

    public override string ToString() {
        return $"EnterMethod {Target.Name} {RegisterCount}";
    }    
}