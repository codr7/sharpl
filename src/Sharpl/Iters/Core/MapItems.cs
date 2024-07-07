namespace Sharpl.Iters.Core;

public class MapItems : BasicIter
{
    public readonly (Value, Value)[] Items;
    public readonly int MaxIndex;
    private int index;

    public MapItems((Value, Value)[] items, int minIndex, int maxIndex)
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
            return Value.Make(Libs.Core.Pair, Items[index]);
        }

        return null;
    }
}