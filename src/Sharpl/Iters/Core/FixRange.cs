namespace Sharpl.Iters.Core;

public class FixRange : Iter
{
    public readonly long? MaxVal;
    public readonly ulong Stride;
    private ulong value;

    public FixRange(ulong min, ulong? max, ulong stride)
    {
        MaxVal = (max == null) ? null : Fix.Val((ulong)max);
        Stride = stride;
        value = Fix.Subtract(min, stride);
    }

    public override Value? Next(VM vm, Loc loc)
    {
        var v = Fix.Add(value, Stride);

        if (Fix.Val(v) < MaxVal)
        {
            value = v;
            return Value.Make(Libs.Core.Fix, value);
        }

        return null;
    }
}