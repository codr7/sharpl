namespace Sharpl.Types.Core;

public class IntType : ComparableType<int>, NumericTrait, RangeTrait
{
    public IntType(string name) : base(name) { }

    public override bool Bool(Value value)
    {
        return value.Cast(this) != 0;
    }

    public Iter CreateRange(Loc loc, Value min, Value max, Value stride)
    {
        int minVal = (min.Type == Libs.Core.Nil) ? 0 : min.TryCast(loc, this);
        int? maxVal = (max.Type == Libs.Core.Nil) ? null : max.TryCast(loc, this);
        int strideVal = (stride.Type == Libs.Core.Nil) ? ((maxVal is int mv && maxVal < minVal) ? -1 : 1) : stride.TryCast(loc, this);
        return new Iters.Core.IntRange(minVal, maxVal, strideVal);
    }

    public void Add(Loc loc, VM vm, Stack stack, int arity)
    {
        var res = 0;

        while (arity > 0)
        {
            res += stack.Pop().TryCast(loc, this);
            arity--;
        }

        stack.Push(this, res);
    }

    public void Divide(Loc loc, VM vm, Stack stack, int arity)
    {
        stack.Reverse(arity);
        var res = stack.Pop().TryCast(loc, this);
        arity--;

        while (arity > 0)
        {
            res /= stack.Pop().TryCast(loc, this);
            arity--;
        }

        stack.Push(this, res);
    }

    public void Multiply(Loc loc, VM vm, Stack stack, int arity)
    {
        var res = stack.Pop().TryCast(loc, this);
        arity--;

        while (arity > 0)
        {
            res *= stack.Pop().TryCast(loc, this);
            arity--;
        }

        stack.Push(this, res);
    }

    public void Subtract(Loc loc, VM vm, Stack stack, int arity)
    {
        var res = 0;

        if (arity > 0)
        {
            if (arity == 1)
            {
                res = -stack.Pop().TryCast(loc, this);

            }
            else
            {
                stack.Reverse(arity);
                res = stack.Pop().TryCast(loc, this);
                arity--;

                while (arity > 0)
                {
                    res -= stack.Pop().TryCast(loc, this);
                    arity--;
                }
            }
        }

        stack.Push(this, res);
    }
}