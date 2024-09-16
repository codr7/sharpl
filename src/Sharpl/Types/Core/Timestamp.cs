using System.Data;
using System.Text;

namespace Sharpl.Types.Core;

public class TimestampType(string name) :
    ComparableType<DateTime>(name),
    NumericTrait,
    RangeTrait
{
    public void Add(Loc loc, VM vm, Stack stack, int arity)
    {
        stack.Reverse(arity);
        var res = stack.Pop().CastUnbox(this);
        arity--;

        while (arity > 0)
        {
            res = stack.Pop().CastUnbox(loc, Libs.Core.Duration).AddTo(res);
            arity--;
        }

        stack.Push(this, res);
    }

    public override bool Bool(Value value) => value.CastUnbox(this).CompareTo(DateTime.MinValue) > 0;

    public override void Call(Loc loc, VM vm, Stack stack, int arity) {
        int y = 1, M = 1, d = 1, h = 0, m = 0, s = 0, ms = 0, us = 0;
        
        var get = (int dv) => {
            var v = stack.Pop();
            return (v.Type == Libs.Core.Nil) ? dv : v.CastUnbox(loc, Libs.Core.Int);
        };

        if (arity > 7) { us = get(us); }
        if (arity > 6) { ms = get(ms); }
        if (arity > 5) { s = get(s); }
        if (arity > 4) { m = get(m); }
        if (arity > 3) { h = get(h); }
        if (arity > 2) { d = get(d); }
        if (arity > 1) { M = get(M); }
        if (arity > 0) { y = get(y); }
        stack.Push(Libs.Core.Timestamp, new DateTime(y, M, d, h, m, s, ms, us));
    }

    public Iter CreateRange(Loc loc, Value min, Value max, Value stride)
    {
        DateTime minVal = (min.Type == Libs.Core.Nil) ? DateTime.MinValue : min.CastUnbox(loc, this);
        DateTime maxVal = (max.Type == Libs.Core.Nil) ? DateTime.MaxValue : max.CastUnbox(loc, this);
        
        Duration? strideVal = (stride.Type == Libs.Core.Nil) 
            ? (maxVal is DateTime mv 
                ? new Duration(0, TimeSpan.FromDays((mv.CompareTo(minVal) < 0) ? -1 : 1)) 
                : null) 
            : stride.CastUnbox(loc, Libs.Core.Duration);

        if (strideVal is null) { throw new EvalError(loc, "Missing stride"); }
        return new Iters.Core.TimeRange(minVal, maxVal, (Duration)strideVal);
    }

    public void Divide(Loc loc, VM vm, Stack stack, int arity) =>
        throw new EvalError(loc, "Not supported");

    public void Multiply(Loc loc, VM vm, Stack stack, int arity) =>
            throw new EvalError(loc, "Not supported");

    public void Subtract(Loc loc, VM vm, Stack stack, int arity)
    {
        if (arity == 1) { throw new EvalError(loc, "Not supported"); }
        else if (arity == 2 && stack.Peek().Type == Libs.Core.Timestamp)
        {
            var y = stack.Pop().CastUnbox(this);
            var x = stack.Pop().CastUnbox(this);
            stack.Push(Libs.Core.Duration, new Duration(0, x.Subtract(y)));
        }
        else
        {
            stack.Reverse(arity);
            var res = stack.Pop().CastUnbox(this);
            arity--;

            while (arity > 0)
            {
                res = stack.Pop().CastUnbox(loc, Libs.Core.Duration).SubtractFrom(res);
                arity--;
            }

            stack.Push(this, res);
        }
    }

    public override void Dump(Value value, VM vm, StringBuilder result) => result.Append($"{value.CastUnbox(this):yyyy-MM-dd HH:mm:ss}");

    public override string ToJson(Loc loc, Value value) => $"{value.CastUnbox(this):yyyy-MM-ddTHH:mm:ss.fffZ}";
}