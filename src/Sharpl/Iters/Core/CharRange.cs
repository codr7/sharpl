namespace Sharpl.Iters.Core;

public class CharRange : BasicIter
{
    public readonly char? Max;
    public readonly int Stride;
    private int value;

    public CharRange(char min, char? max, int stride)
    {
        Max = max;
        Stride = stride;
        value = min - stride;
    }

    public override Value? Next()
    {
        if (Max is char mv && value+1 < mv)
        {
            value += Stride;
            return Value.Make(Libs.Core.Int, value);
        }

        return null;
    }
}