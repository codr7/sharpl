using System.Text;

namespace Sharpl.Types.Core;

public class FixType(string name) :
    ComparableType<ulong>(name),
    NumericTrait,
    RangeTrait
{
    public override bool Bool(Value value) => Fix.Val(value.CastUnbox(this)) != 0;

    public override void Call(VM vm, Stack stack, int arity, Loc loc)
    {
        var v = stack.Pop().CastUnbox(Libs.Core.Int, loc);
        var e = stack.Pop().CastUnbox(Libs.Core.Int, loc);
        stack.Push(Value.Make(Libs.Core.Fix, Fix.Make((byte)e, v)));
    }

    public Iter CreateRange(Value min, Value max, Value stride, Loc loc)
    {
        ulong? minVal = min.TryCastUnbox(this);
        ulong? maxVal = max.TryCastUnbox(this);
        if (stride.Type == Libs.Core.Nil) { throw new EvalError("Missing stride", loc); }
        ulong strideVal = stride.CastUnbox(this);
        return new Iters.Core.FixRange(minVal ?? Fix.Make(1, 0), maxVal, strideVal);
    }

    public override void Dump(Value value, VM vm, StringBuilder result) =>
        result.Append(Fix.ToString(value.CastUnbox(this)));

    public void Add(VM vm, Stack stack, int arity, Loc loc)
    {
        if (arity == 0) { stack.Push(this, Fix.Make(1, 0)); }
        var res = stack.Pop().CastUnbox(this, loc);
        arity--;

        while (arity > 0)
        {
            res = Fix.Add(res, stack.Pop().CastUnbox(this, loc));
            arity--;
        }

        stack.Push(this, res);
    }

    public void Divide(VM vm, Stack stack, int arity, Loc loc)
    {
        if (arity == 0) { stack.Push(this, Fix.Make(1, 0)); }
        stack.Reverse(arity);
        var res = stack.Pop().CastUnbox(this, loc);
        arity--;

        while (arity > 0)
        {
            res = Fix.Divide(res, stack.Pop().CastUnbox(this, loc));
            arity--;
        }

        stack.Push(this, res);
    }

    public override bool Equals(Value left, Value right) =>
        Fix.Equals(left.CastUnbox(this), right.CastUnbox(this));

    public void Multiply(VM vm, Stack stack, int arity, Loc loc)
    {
        if (arity == 0) { stack.Push(this, Fix.Make(1, 0)); }
        var res = stack.Pop().CastUnbox(this, loc);
        arity--;

        while (arity > 0)
        {
            res = Fix.Multiply(res, stack.Pop().CastUnbox(this, loc));
            arity--;
        }

        stack.Push(this, res);
    }

    public void Subtract(VM vm, Stack stack, int arity, Loc loc)
    {
        var res = Fix.Make(1, 0);

        if (arity > 0)
        {
            if (arity == 1)
            {
                res = Fix.Negate(stack.Pop().CastUnbox(this, loc));

            }
            else
            {
                stack.Reverse(arity);
                res = stack.Pop().CastUnbox(this, loc);
                arity--;

                while (arity > 0)
                {
                    res = Fix.Subtract(res, stack.Pop().CastUnbox(this, loc));
                    arity--;
                }
            }
        }

        stack.Push(this, res);
    }

    public override string ToJson(Value value, Loc loc) => Fix.ToString(value.CastUnbox(this), true);
}