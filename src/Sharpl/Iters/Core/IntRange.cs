namespace Sharpl.Iters.Core;

public class IntRange : BasicIter
{
    public readonly int? Max;
    public readonly int Stride;
    private int value;

    public IntRange(int min, int? max, int stride)
    {
        Max = max;
        Stride = stride;
        value = min - stride;
    }

    public override Value? Next()
    {
        if (Max is int mv && value+1 < mv)
        {
            value += Stride;
            return Value.Make(Libs.Core.Int, value);
        }

        return null;
    }
}