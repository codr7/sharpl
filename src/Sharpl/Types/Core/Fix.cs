using System.Text;

namespace Sharpl.Types.Core;

public class FixType : ComparableType<ulong>, NumericTrait, RangeTrait
{
    public FixType(string name) : base(name) { }

    public override bool Bool(Value value)
    {
        return Fix.Val(value.Cast(this)) != 0;
    }

    public override void Call(Loc loc, VM vm, Stack stack, int arity)
    {
        var v = stack.Pop().TryCast(loc, Libs.Core.Int);
        var e = stack.Pop().TryCast(loc, Libs.Core.Int);
        stack.Push(Value.Make(Libs.Core.Fix, Fix.Make((byte)e, v)));
    }

    public Iter CreateRange(Loc loc, Value min, Value max, Value stride)
    {
        ulong? minVal = min.TryCast(this);
        ulong? maxVal = max.TryCast(this);
        ulong strideVal = stride.Cast(this);

        return new Iters.Core.FixRange(minVal ?? Fix.Make(1, 0), maxVal, strideVal);
    }

    public override void Dump(Value value, StringBuilder result)
    {
        result.Append(Fix.ToString(value.Cast(this)));
    }

    public void Add(Loc loc, VM vm, Stack stack, int arity)
    {
        if (arity == 0) {
            stack.Push(this, Fix.Make(1, 0));
        }

        var res = stack.Pop().TryCast(loc, this);
        arity--;

        while (arity > 0)
        {
            res = Fix.Add(res, stack.Pop().TryCast(loc, this));
            arity--;
        }

        stack.Push(this, res);
    }

    public void Divide(Loc loc, VM vm, Stack stack, int arity)
    {
        if (arity == 0) {
            stack.Push(this, Fix.Make(1, 0));
        }

        stack.Reverse(arity);
        var res = stack.Pop().TryCast(loc, this);
        arity--;

        while (arity > 0)
        {
            res = Fix.Divide(res, stack.Pop().TryCast(loc, this));
            arity--;
        }

        stack.Push(this, res);
    }

    public override bool Equals(Value left, Value right)
    {
        return Fix.Equals(left.Cast(this), right.Cast(this));
    }

    public void Multiply(Loc loc, VM vm, Stack stack, int arity)
    {        
        if (arity == 0) {
            stack.Push(this, Fix.Make(1, 0));
        }

        var res = stack.Pop().TryCast(loc, this);
        arity--;

        while (arity > 0)
        {
            res = Fix.Multiply(res, stack.Pop().TryCast(loc, this));
            arity--;
        }

        stack.Push(this, res);
    }

    public void Subtract(Loc loc, VM vm, Stack stack, int arity)
    {
        var res = Fix.Make(1, 0);

        if (arity > 0)
        {
            if (arity == 1)
            {
                res = Fix.Negate(stack.Pop().TryCast(loc, this));

            }
            else
            {
                stack.Reverse(arity);
                res = stack.Pop().TryCast(loc, this);
                arity--;

                while (arity > 0)
                {
                    res = Fix.Subtract(res, stack.Pop().TryCast(loc, this));
                    arity--;
                }
            }
        }

        stack.Push(this, res);
    }
}