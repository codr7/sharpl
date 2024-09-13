using System.Text;

namespace Sharpl.Types.Core;

public class TimestampType(string name) :
    ComparableType<DateTime>(name),
    NumericTrait,
    RangeTrait
{
    public void Add(Loc loc, VM vm, Stack stack, int arity)
    {
        /*var res = 0;

        while (arity > 0)
        {
            res += stack.Pop().CastUnbox(loc, this);
            arity--;
        }

        stack.Push(this, res);*/
    }

    public override bool Bool(Value value) => value.CastUnbox(this).CompareTo(DateTime.MinValue) > 0;

    public Iter CreateRange(Loc loc, Value min, Value max, Value stride)
    {
        DateTime minVal = (min.Type == Libs.Core.Nil) ? DateTime.MinValue : min.CastUnbox(loc, this);
        DateTime maxVal = (max.Type == Libs.Core.Nil) ? DateTime.MaxValue : max.CastUnbox(loc, this);
        TimeSpan? strideVal = (stride.Type == Libs.Core.Nil) ? (maxVal is DateTime mv ? TimeSpan.FromDays((mv.CompareTo(minVal) < 0) ? -1 : 1) : null) : stride.CastUnbox(loc, Libs.Core.Duration);
        if (strideVal is null) { throw new EvalError(loc, "Missing stride"); }
        return new Iters.Core.TimeRange(minVal, maxVal, (TimeSpan)strideVal);
    }

    public void Divide(Loc loc, VM vm, Stack stack, int arity)
    {
        /*stack.Reverse(arity);
        var res = stack.Pop().CastUnbox(loc, this);
        arity--;

        while (arity > 0)
        {
            res /= stack.Pop().CastUnbox(loc, this);
            arity--;
        }

        stack.Push(this, res);*/
    }

    public void Multiply(Loc loc, VM vm, Stack stack, int arity)
    {
        /*var res = stack.Pop().CastUnbox(loc, this);
        arity--;

        while (arity > 0)
        {
            res *= stack.Pop().CastUnbox(loc, this);
            arity--;
        }

        stack.Push(this, res);*/
    }

    public void Subtract(Loc loc, VM vm, Stack stack, int arity)
    {
        /*var res = 0;

        if (arity > 0)
        {
            if (arity == 1) { res = -stack.Pop().CastUnbox(loc, this); }
            else
            {
                stack.Reverse(arity);
                res = stack.Pop().CastUnbox(loc, this);
                arity--;

                while (arity > 0)
                {
                    res -= stack.Pop().CastUnbox(loc, this);
                    arity--;
                }
            }
        }

        stack.Push(this, res);*/
    }

    public override void Dump(Value value, VM vm, StringBuilder result) => result.Append($"{value.CastUnbox(this):yyyy-MM-dd HH:mm:ss}");

    public override string ToJson(Loc loc, Value value) => $"{value.CastUnbox(this):yyyy-MM-ddTHH:mm:ss.fffZ}";
}