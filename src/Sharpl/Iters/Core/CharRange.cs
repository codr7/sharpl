namespace Sharpl.Iters.Core;

public class CharRange : Iter
{
    public readonly char? Max;
    public readonly int Stride;
    private char value;

    public CharRange(char min, char? max, int stride)
    {
        Max = max;
        Stride = stride;
        value = (char)(min - (char)stride);
    }

    public override Value? Next(VM vm, Loc loc)
    {
        if (Max is char mv && value + 1 < mv)
        {
            value += (char)Stride;
            return Value.Make(Libs.Core.Char, value);
        }

        return null;
    }
}