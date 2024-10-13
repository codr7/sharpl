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

    public override bool Next(VM vm, Register result, Loc loc)
    {
        var nv = Stride.AddTo(value);

        if (Max is null || nv.CompareTo(Max) < 0)
        {
            value = nv;
            vm.Set(result, Value.Make(Libs.Core.Timestamp, value));
            return true;
        }

        return false;
    }

    public override string Dump(VM vm) =>
        $"(range {Min} {((Max is null) ? "_" : Max)} {Stride})";
}