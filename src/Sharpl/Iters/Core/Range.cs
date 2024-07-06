namespace Sharpl.Iters.Core;

public class Range : BasicIter
{
    public readonly int Max;
    private int current;

    public Range(int min, int max)
    {
        Max = max;
        current = min - 1;
    }

    public override Value? Next()
    {
        if (current+1 < Max)
        {
            current++;
            return Value.Make(Libs.Core.Int, current);
        }

        return null;
    }
}