namespace Sharpl.Iters.Core;

public class TimeRange : Iter
{
    public readonly DateTime Min;
    public readonly DateTime? Max;
    public readonly Duration Stride;
    private DateTime value;

    public TimeRange(DateTime min, DateTime? max, Duration stride)
    {
        Min = min;
        Max = max;
        Stride = stride;
        value = stride.SubtractFrom(min);
    }

    public override Value? Next(VM vm, Loc loc)
    {
        var nv = Stride.AddTo(value);

        if (Max is null || nv.CompareTo(Max) < 0)
        {
            value = nv;
            return Value.Make(Libs.Core.Timestamp, value);
        }

        return null;
    }

    public override string Dump(VM vm) =>
        $"(range {Min} {((Max is null) ? "_" : Max)} {Stride})";
}