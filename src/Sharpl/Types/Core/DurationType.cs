using System.Text;

namespace Sharpl.Types.Core;

public class DurationType(string name) :
    ComparableType<TimeSpan>(name),
    NumericTrait
{
    public static readonly TimeSpan ZERO = TimeSpan.FromTicks(0);

    public void Add(Loc loc, VM vm, Stack stack, int arity)
    {
        var res = TimeSpan.FromTicks(0);

        while (arity > 0)
        {
            res = res.Add(stack.Pop().CastUnbox(loc, this));
            arity--;
        }

        stack.Push(this, res);
    }

    public override bool Bool(Value value) => value.CastUnbox(this).CompareTo(ZERO)> 0;

    public void Divide(Loc loc, VM vm, Stack stack, int arity)
    {
        stack.Reverse(arity);
        var res = stack.Pop().CastUnbox(loc, this);
        arity--;

        while (arity > 0)
        {
            res = res.Divide(stack.Pop().CastUnbox(loc, Libs.Core.Int));
            arity--;
        }

        stack.Push(this, res);
    }

    public void Multiply(Loc loc, VM vm, Stack stack, int arity)
    {
        var res = stack.Pop().CastUnbox(loc, this);
        arity--;

        while (arity > 0)
        {
            res = res.Multiply(stack.Pop().CastUnbox(loc, Libs.Core.Int));
            arity--;
        }

        stack.Push(this, res);
    }

    public void Subtract(Loc loc, VM vm, Stack stack, int arity)
    {
        var res = TimeSpan.FromTicks(0);

        if (arity > 0)
        {
            if (arity == 1) { res = res.Subtract(stack.Pop().CastUnbox(loc, this)); }
            else
            {
                stack.Reverse(arity);
                res = stack.Pop().CastUnbox(loc, this);
                arity--;

                while (arity > 0)
                {
                    res = res.Subtract(stack.Pop().CastUnbox(loc, this));
                    arity--;
                }
            }
        }

        stack.Push(this, res);
    }

    public override void Say(Value value, VM vm, StringBuilder result) => result.Append(value.CastUnbox(this));
}