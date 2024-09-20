using System.Text;

namespace Sharpl.Types.Core;

public class FixType(string name) :
    ComparableType<ulong>(name),
    NumericTrait,
    RangeTrait
{
    public override bool Bool(Value value) => Fix.Val(value.CastUnbox(this)) != 0;

    public override void Call(Loc loc, VM vm, Stack stack, int arity)
    {
        var v = stack.Pop().CastUnbox(loc, Libs.Core.Int);
        var e = stack.Pop().CastUnbox(loc, Libs.Core.Int);
        stack.Push(Value.Make(Libs.Core.Fix, Fix.Make((byte)e, v)));
    }

    public Iter CreateRange(Loc loc, Value min, Value max, Value stride)
    {
        ulong? minVal = min.TryCastUnbox(this);
        ulong? maxVal = max.TryCastUnbox(this);
        if (stride.Type == Libs.Core.Nil) { throw new EvalError("Missing stride", loc); }
        ulong strideVal = stride.CastUnbox(this);
        return new Iters.Core.FixRange(minVal ?? Fix.Make(1, 0), maxVal, strideVal);
    }

    public override void Dump(Value value, VM vm, StringBuilder result) => 
        result.Append(Fix.ToString(value.CastUnbox(this)));

    public void Add(Loc loc, VM vm, Stack stack, int arity)
    {
        if (arity == 0) { stack.Push(this, Fix.Make(1, 0)); }
        var res = stack.Pop().CastUnbox(loc, this);
        arity--;

        while (arity > 0)
        {
            res = Fix.Add(res, stack.Pop().CastUnbox(loc, this));
            arity--;
        }

        stack.Push(this, res);
    }

    public void Divide(Loc loc, VM vm, Stack stack, int arity)
    {
        if (arity == 0) { stack.Push(this, Fix.Make(1, 0)); }
        stack.Reverse(arity);
        var res = stack.Pop().CastUnbox(loc, this);
        arity--;

        while (arity > 0)
        {
            res = Fix.Divide(res, stack.Pop().CastUnbox(loc, this));
            arity--;
        }

        stack.Push(this, res);
    }

    public override bool Equals(Value left, Value right) =>
        Fix.Equals(left.CastUnbox(this), right.CastUnbox(this));

    public void Multiply(Loc loc, VM vm, Stack stack, int arity)
    {        
        if (arity == 0) { stack.Push(this, Fix.Make(1, 0)); }
        var res = stack.Pop().CastUnbox(loc, this);
        arity--;

        while (arity > 0)
        {
            res = Fix.Multiply(res, stack.Pop().CastUnbox(loc, this));
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
                res = Fix.Negate(stack.Pop().CastUnbox(loc, this));

            }
            else
            {
                stack.Reverse(arity);
                res = stack.Pop().CastUnbox(loc, this);
                arity--;

                while (arity > 0)
                {
                    res = Fix.Subtract(res, stack.Pop().CastUnbox(loc, this));
                    arity--;
                }
            }
        }

        stack.Push(this, res);
    }

    public override string ToJson(Loc loc, Value value) => Fix.ToString(value.CastUnbox(this), true);
}