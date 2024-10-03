using System.Text;

namespace Sharpl.Types.Core;

public class DurationType(string name, AnyType[] parents) :
    ComparableType<Duration>(name, parents), NumericTrait
{
    public static readonly Duration ZERO = new Duration(0, TimeSpan.FromTicks(0));

    public void Add(VM vm, Stack stack, int arity, Loc loc)
    {
        var res = new Duration(0, TimeSpan.FromTicks(0));

        while (arity > 0)
        {
            res = stack.Pop().CastUnbox(this, loc).Add(res);
            arity--;
        }

        stack.Push(this, res);
    }

    public override bool Bool(Value value) => value.CastUnbox(this).CompareTo(ZERO) > 0;

    public void Divide(VM vm, Stack stack, int arity, Loc loc)
    {
        stack.Reverse(arity);
        var res = stack.Pop().CastUnbox(this, loc);
        arity--;

        while (arity > 0)
        {
            res = res.Divide(stack.Pop().CastUnbox(Libs.Core.Int, loc));
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
            res = res.Multiply(stack.Pop().CastUnbox(Libs.Core.Int, loc));
            arity--;
        }

        stack.Push(this, res);
    }

    public void Subtract(VM vm, Stack stack, int arity, Loc loc)
    {
        var res = new Duration(0, TimeSpan.FromTicks(0));

        if (arity > 0)
        {
            if (arity == 1) { res = res.Subtract(stack.Pop().CastUnbox(this, loc)); }
            else
            {
                stack.Reverse(arity);
                res = stack.Pop().CastUnbox(this, loc);
                arity--;

                while (arity > 0)
                {
                    res = res.Subtract(stack.Pop().CastUnbox(this, loc));
                    arity--;
                }
            }
        }

        stack.Push(this, res);
    }

    public override void Say(Value value, VM vm, StringBuilder result) => result.Append(value.CastUnbox(this));
}