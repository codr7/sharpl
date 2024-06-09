namespace Sharpl.Ops;

public readonly record struct SetArrayItem(int Index)
{
    public static Op Make(int index)
    {
        return new Op(Op.T.SetArrayItem, new SetArrayItem(index));
    }

    public override string ToString() {
        return $"(set-array-item {Index})";
    }    
}