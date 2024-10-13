namespace Sharpl.Iters.Core;

public class IntRange : Iter
{
    public readonly int Min;
    public readonly int? Max;
    public readonly int Stride;
    private int value;

    public IntRange(int min, int? max, int stride)
    {
        Min = min;
        Max = max;
        Stride = stride;
        value = min - stride;
    }

    public override bool Next(VM vm, Register result, Loc loc)
    {
        if (Max is null || value + 1 < Max)
        {
            value += Stride;
            vm.Set(result, Value.Make(Libs.Core.Int, value));
            return true;
        }

        return false;
    }

    public override string Dump(VM vm) => $"(range {Min} {((Max is null) ? "_" : Max)} {Stride})";
}