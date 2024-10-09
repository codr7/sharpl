using System.Text;

namespace Sharpl.Types.Core;

public class TimestampType(string name, AnyType[] parents) :
    ComparableType<DateTime>(name, parents), NumericTrait, RangeTrait
{
    public void Add(VM vm, int arity, Register result, Loc loc)
    {
        var res = vm.GetRegister(0, 0).CastUnbox(this);
        for (var i = 1; i < arity; i++) { res = vm.GetRegister(0, i).CastUnbox(Libs.Core.Duration, loc).AddTo(res); }
        vm.Set(result, Value.Make(this, res));
    }

    public override bool Bool(Value value) => value.CastUnbox(this).CompareTo(DateTime.MinValue) > 0;

    public override void Call(VM vm, int arity, Register result, Loc loc)
    {
        int y = 1, M = 1, d = 1, h = 0, m = 0, s = 0, ms = 0, us = 0;

        var get = (int i, int dv) =>
        {
            var v = vm.GetRegister(0, i);
            return (v.Type == Libs.Core.Nil) ? dv : v.CastUnbox(Libs.Core.Int, loc);
        };

        if (arity > 0) { y = get(0, y); }
        if (arity > 1) { M = get(1, M); }
        if (arity > 2) { d = get(2, d); }
        if (arity > 3) { h = get(3, h); }
        if (arity > 4) { m = get(4, m); }
        if (arity > 5) { s = get(5, s); }
        if (arity > 6) { ms = get(6, ms); }
        if (arity > 7) { us = get(7, us); }
        vm.Set(result, Value.Make(Libs.Core.Timestamp, new DateTime(y, M, d, h, m, s, ms, us)));
    }

    public Iter CreateRange(Value min, Value max, Value stride, Loc loc)
    {
        DateTime minVal = (min.Type == Libs.Core.Nil) ? DateTime.MinValue : min.CastUnbox(this, loc);
        DateTime maxVal = (max.Type == Libs.Core.Nil) ? DateTime.MaxValue : max.CastUnbox(this, loc);

        Duration? strideVal = (stride.Type == Libs.Core.Nil)
            ? (maxVal is DateTime mv
                ? new Duration(0, TimeSpan.FromDays((mv.CompareTo(minVal) < 0) ? -1 : 1))
                : null)
            : stride.CastUnbox(Libs.Core.Duration, loc);

        if (strideVal is null) { throw new EvalError("Missing stride", loc); }
        return new Iters.Core.TimeRange(minVal, maxVal, (Duration)strideVal);
    }

    public void Divide(VM vm, int arity, Register result, Loc loc) =>
        throw new EvalError("Not supported", loc);

    public void Multiply(VM vm, int arity, Register result, Loc loc) =>
            throw new EvalError("Not supported", loc);

    public void Subtract(VM vm, int arity, Register result, Loc loc)
    {
        if (arity == 1) { throw new EvalError("Not supported", loc); }
        else if (arity == 2 && vm.GetRegister(0, 1).Type == Libs.Core.Timestamp)
        {
            var x = vm.GetRegister(0, 0).CastUnbox(this);
            var y = vm.GetRegister(0, 1).CastUnbox(this);
            vm.Set(result, Value.Make(Libs.Core.Duration, new Duration(0, x.Subtract(y))));
        }
        else
        {
            var res = vm.GetRegister(0, 0).CastUnbox(this);

            for (var i = 1; i < arity; i++)
                res = vm.GetRegister(0, i).CastUnbox(Libs.Core.Duration, loc).SubtractFrom(res);

            vm.Set(result, Value.Make(this, res));
        }
    }

    public override void Dump(VM vm, Value value, StringBuilder result) => 
        result.Append($"{value.CastUnbox(this):yyyy-MM-dd HH:mm:ss}");

    public override string ToJson(Value value, Loc loc) => 
        $"{value.CastUnbox(this):yyyy-MM-ddTHH:mm:ss.fffZ}";
}