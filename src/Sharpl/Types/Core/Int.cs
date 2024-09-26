namespace Sharpl.Types.Core;

public class IntType(string name) :
    ComparableType<int>(name),
    NumericTrait,
    RangeTrait
{
    public override bool Bool(Value value) => value.CastUnbox(this) != 0;

    public Iter CreateRange(Value min, Value max, Value stride, Loc loc)
    {
        int minVal = (min.Type == Libs.Core.Nil) ? 0 : min.CastUnbox(this, loc);
        int? maxVal = (max.Type == Libs.Core.Nil) ? null : max.CastUnbox(this, loc);
        int strideVal = (stride.Type == Libs.Core.Nil) ? ((maxVal is int mv && maxVal < minVal) ? -1 : 1) : stride.CastUnbox(this, loc);
        return new Iters.Core.IntRange(minVal, maxVal, strideVal);
    }

    public void Add(VM vm, Stack stack, int arity, Loc loc)
    {
        var res = 0;

        while (arity > 0)
        {
            res += stack.Pop().CastUnbox(this, loc);
            arity--;
        }

        stack.Push(this, res);
    }

    public void Divide(VM vm, Stack stack, int arity, Loc loc)
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

    public void Multiply(VM vm, Stack stack, int arity, Loc loc)
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

    public void Subtract(VM vm, Stack stack, int arity, Loc loc)
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