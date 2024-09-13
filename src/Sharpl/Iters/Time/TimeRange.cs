namespace Sharpl.Iters.Core;

public class TimeRange : BasicIter
{
    public readonly DateTime Min;
    public readonly DateTime? Max;
    public readonly TimeSpan Stride;
    private DateTime value;

    public TimeRange(DateTime min, DateTime? max, TimeSpan stride)
    {
        Min = min;
        Max = max;
        Stride = stride;
        value = min - stride;
    }

    public override Value? Next()
    {
        if (Max is null || value.Add(Stride).CompareTo(Max) < 0)
        {
            value = value.Add(Stride);
            return Value.Make(Libs.Core.Timestamp, value);
        }

        return null;
    }

    public override string Dump(VM vm) =>
        $"(range {Min} {((Max is null) ? "_" : Max)} {Stride})";
}