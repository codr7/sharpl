namespace Sharpl.Types.Core;

public class IntType : ComparableType<int>, IterableTrait, NumericTrait
{
    public IntType(string name) : base(name) { }

    public override bool Bool(Value value)
    {
        return value.Cast(this) != 0;
    }

    public Iter CreateIter(Value target)
    {
        return new Iters.Core.Range(0, target.Cast(this));
    }

    public void Add(Loc loc, VM vm, Stack stack, int arity)
    {
        var res = 0;

        while (arity > 0)
        {
            res += stack.Pop().Cast(loc, this);
            arity--;
        }

        stack.Push(this, res);
    }

    public void Divide(Loc loc, VM vm, Stack stack, int arity)
    {
        stack.Reverse(arity);
        var res = stack.Pop().Cast(loc, this);
        arity--;

        while (arity > 0)
        {
            res /= stack.Pop().Cast(loc, this);
            arity--;
        }

        stack.Push(this, res);
    }

    public void Multiply(Loc loc, VM vm, Stack stack, int arity)
    {
        var res = stack.Pop().Cast(loc, this);
        arity--;

        while (arity > 0)
        {
            res *= stack.Pop().Cast(loc, this);
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
                res = -stack.Pop().Cast(loc, this);

            }
            else
            {
                stack.Reverse(arity);
                res = stack.Pop().Cast(loc, this);
                arity--;

                while (arity > 0)
                {
                    res -= stack.Pop().Cast(loc, this);
                    arity--;
                }
            }
        }

        stack.Push(this, res);
    }
}