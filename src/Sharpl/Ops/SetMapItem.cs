namespace Sharpl.Ops;

public readonly record struct SetMapItem()
{
    public static Op Make()
    {
        return new Op(Op.T.SetMapItem, new SetMapItem());
    }

    public override string ToString() {
        return $"SetMapItem";
    }    
}