namespace Sharpl.Types.Core;

public class IntType(string name) :
    ComparableType<int>(name),
    NumericTrait,
    RangeTrait
{
    public override bool Bool(Value value) => value.CastUnbox(this) != 0;

    public Iter CreateRange(Loc loc, Value min, Value max, Value stride)
    {
        int minVal = (min.Type == Libs.Core.Nil) ? 0 : min.CastUnbox(this, loc);
        int? maxVal = (max.Type == Libs.Core.Nil) ? null : max.CastUnbox(this, loc);
        int strideVal = (stride.Type == Libs.Core.Nil) ? ((maxVal is int mv && maxVal < minVal) ? -1 : 1) : stride.CastUnbox(this, loc);
        return new Iters.Core.IntRange(minVal, maxVal, strideVal);
    }

    public void Add(Loc loc, VM vm, Stack stack, int arity)
    {
        var res = 0;

        while (arity > 0)
        {
            res += stack.Pop().CastUnbox(this, loc);
            arity--;
        }

        stack.Push(this, res);
    }

    public void Divide(Loc loc, VM vm, Stack stack, int arity)
    {
        stack.Reverse(arity);
        var res = stack.Pop().CastUnbox(this, loc);
        arity--;

        while (arity > 0)
        {
            res /= stack.Pop().CastUnbox(this, loc);
            arity--;
        }

        stack.Push(this, res);
    }

    public void Multiply(Loc loc, VM vm, Stack stack, int arity)
    {
        var res = stack.Pop().CastUnbox(this, loc);
        arity--;

        while (arity > 0)
        {
            res *= stack.Pop().CastUnbox(this, loc);
            arity--;
        }

        stack.Push(this, res);
    }

    public void Subtract(Loc loc, VM vm, Stack stack, int arity)
    {
        var res = 0;

        if (arity > 0)
        {
            if (arity == 1) { res = -stack.Pop().CastUnbox(this, loc); }
            else
            {
                stack.Reverse(arity);
                res = stack.Pop().CastUnbox(this, loc);
                arity--;

                while (arity > 0)
                {
                    res -= stack.Pop().CastUnbox(this, loc);
                    arity--;
                }
            }
        }

        stack.Push(this, res);
    }

    public override string ToJson(Value value, Loc loc) => $"{value.CastUnbox(this)}";
}