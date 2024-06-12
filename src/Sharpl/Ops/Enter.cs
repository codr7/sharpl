namespace Sharpl.Ops;

public readonly record struct Enter(UserMethod Target)
{
    public static Op Make(UserMethod target)
    {
        return new Op(Op.T.Enter, new Enter(target));
    }

    public override string ToString() {
        return $"Enter {Target.Name}";
    }    
}