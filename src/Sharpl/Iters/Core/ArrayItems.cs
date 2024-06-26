namespace Sharpl.Iters.Core;

public class ArrayItems : BasicIter
{
    public readonly Value[] Items;
    public readonly int MaxIndex;
    private int index;

    public ArrayItems(Value[] items, int minIndex, int maxIndex)
    {
        Items = items;
        MaxIndex = maxIndex;
        index = minIndex - 1;
    }

    public override Value? Next()
    {
        if (index+1 < MaxIndex)
        {
            index++;
            return Items[index];
        }

        return null;
    }
}