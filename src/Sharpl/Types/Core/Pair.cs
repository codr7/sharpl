using Sharpl.Iters.Core;
using System.Text;

namespace Sharpl.Types.Core;

using Sharpl.Libs;

public class PairType(string name, AnyType[] parents) : 
    Type<(Value, Value)>(name, parents), ComparableTrait, IterTrait, LengthTrait, StackTrait
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

        throw new EvalError("Index out of bounds", loc);
    }

    public override void Call(VM vm, Stack stack, int arity, Loc loc)
    {
        if (arity < 2) { throw new EvalError("Wrong number of arguments", loc); }
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

    public override void Call(VM vm, Stack stack, Value target, int arity, int registerCount, bool eval, Loc loc)
    {
        switch (arity)
        {
            case 1:
                {
                    var t = target;

                    for (var i = stack.Pop().CastUnbox(Core.Int, loc); i >= 0; i--)
                    {
                        switch (i)
                        {
                            case 0:
                                stack.Push(t.CastUnbox(this, loc).Item1);
                                break;
                            case 1:
                                stack.Push(t.CastUnbox(this, loc).Item2);
                                return;
                            default:
                                t = t.CastUnbox(this, loc).Item2;
                                break;
                        }
                    }

                    break;
                }
            case 2:
                {
                    var v = stack.Pop();
                    var i = stack.Pop().CastUnbox(Core.Int, loc);
                    stack.Push(Update(loc, target, v, i));
                    break;
                }
            default:
                throw new EvalError($"Wrong number of arguments: {arity}", loc);
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

    public Sharpl.Iter CreateIter(Value target, VM vm, Loc loc) => new PairItems(target);

    public override Value Copy(Value value)
    {
        var p = value.CastUnbox(this);
        return Value.Make(this, (p.Item1.Copy(), p.Item2.Copy()));
    }

    public override void Dump(VM vm, Value value, StringBuilder result)
    {
        var p = value.CastUnbox(this);
        p.Item1.Dump(vm, result);
        result.Append(':');
        p.Item2.Dump(vm, result);
    }

    public override bool Equals(Value left, Value right)
    {
        var lp = left.CastUnbox(this);
        var rp = right.CastUnbox(this);
        return lp.Item1.Equals(rp.Item1) && lp.Item2.Equals(rp.Item2);
    }

    public int Length(Value target)
    {
        var v = target;
        var result = 1;

        while (v.Type == this)
        {
            var p = v.CastUnbox(this);
            v = p.Item2;
            result++;
        }

        return result;
    }

    public Value Peek(Loc loc, VM vm, Value srcVal) => srcVal.CastUnbox(this).Item1;

    public Value Pop(Loc loc, VM vm, Register src, Value srcVal)
    {
        var sv = srcVal.CastUnbox(this);
        vm.Set(src, sv.Item2);
        return sv.Item1;
    }

    public void Push(Loc loc, VM vm, Register dst, Value dstVal, Value val) =>
        vm.Set(dst, Value.Make(this, (val, dstVal)));

    public override void Say(VM vm, Value value, StringBuilder result)
    {
        var p = value.CastUnbox(this);
        p.Item1.Say(vm, result);
        result.Append(':');
        p.Item2.Say(vm, result);
    }
}