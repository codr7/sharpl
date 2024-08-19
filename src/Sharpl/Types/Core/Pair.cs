using Sharpl.Iters.Core;
using System.Text;

namespace Sharpl.Types.Core;

using Sharpl.Libs;

public class PairType : Type<(Value, Value)>, ComparableTrait, IterTrait
{
    public static Value Update(Loc loc, Value target, Value value, int i)
    {
        if (i == 0)
        {
            return (target.Type == Core.Pair)
                ? Value.Make(Core.Pair, (value, target.CastUnbox(Core.Pair).Item2))
                : value;
        }

        if (target.Type == Core.Pair)
        {
            var p = target.CastUnbox(Core.Pair);
            return Value.Make(Core.Pair, (p.Item1, Update(loc, p.Item2, value, i - 1)));
        }

        throw new EvalError(loc, "Index out of bounds");
    }

    public PairType(string name) : base(name) { }

    public override void Call(Loc loc, VM vm, Stack stack, int arity)
    {
        if (arity < 2) { throw new EvalError(loc, "Wrong number of arguments"); }
        var r = stack.Pop();
        arity--;

        while (arity > 0)
        {
            var l = stack.Pop();
            r = Value.Make(Core.Pair, (l, r));
            arity--;
        }

        stack.Push(r);
    }

    public override void Call(Loc loc, VM vm, Stack stack, Value target, int arity, int registerCount)
    {
        switch (arity)
        {
            case 1:
                {
                    var t = target;

                    for (var i = stack.Pop().CastUnbox(loc, Core.Int); i >= 0; i--)
                    {
                        switch (i)
                        {
                            case 0:
                                stack.Push(target.CastUnbox(loc, this).Item1);
                                break;
                            case 1:
                                stack.Push(target.CastUnbox(loc, this).Item2);
                                break;
                            default:
                                t = target.CastUnbox(loc, this).Item2;
                                break;
                        }
                    }

                    break;
                }
            case 2:
                {
                    var v = stack.Pop();
                    var i = stack.Pop().CastUnbox(loc, Core.Int);
                    stack.Push(Update(loc, target, v, i));
                    break;
                }
            default:
                throw new EvalError(loc, $"Wrong number of arguments: {arity}");
        }
    }

    public Order Compare(Value left, Value right)
    {
        var lp = left.CastUnbox(this);
        var rp = right.CastUnbox(this);

        if (lp.Item1.Type is ComparableTrait t)
        {
            var res = t.Compare(lp.Item1, rp.Item1);
            return (res == Order.EQ) ? t.Compare(lp.Item2, rp.Item2) : res;
        }

        return Order.EQ;
    }

    public override void Dump(Value value, StringBuilder result)
    {
        var p = value.CastUnbox(this);
        p.Item1.Dump(result);
        result.Append(':');
        p.Item2.Dump(result);
    }

    public override bool Equals(Value left, Value right)
    {
        var lp = left.CastUnbox(this);
        var rp = right.CastUnbox(this);
        return lp.Item1.Equals(rp.Item1) && lp.Item2.Equals(rp.Item2);
    }

    public Sharpl.Iter CreateIter(Value target) => new PairItems(target);

    public override void Say(Value value, StringBuilder result)
    {
        var p = value.CastUnbox(this);
        p.Item1.Say(result);
        result.Append(':');
        p.Item2.Say(result);
    }
}