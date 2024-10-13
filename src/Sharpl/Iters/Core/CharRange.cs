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

    public override bool Next(VM vm, Register result, Loc loc)
    {
        if (Max is char mv && value + 1 < mv)
        {
            value += (char)Stride;
            vm.Set(result, Value.Make(Libs.Core.Char, value));
            return true;
        }

        return false;
    }
}