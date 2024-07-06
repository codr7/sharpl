namespace Sharpl.Ops;

public readonly record struct AddMapItem()
{
    public static Op Make()
    {
        return new Op(Op.T.AddMapItem, new AddMapItem());
    }

    public override string ToString() {
        return $"AddMapItem";
    }    
}