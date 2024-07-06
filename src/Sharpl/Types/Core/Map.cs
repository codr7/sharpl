namespace Sharpl.Types.Core;

using Sharpl.Iters.Core;
using Sharpl.Libs;
using System.Text;

public class MapType : Type<OrderedMap<Value, Value>>, ComparableTrait/*, IterableTrait*/
{
    public MapType(string name) : base(name) { }

    public override bool Bool(Value value)
    {
        return value.Cast(this).Count != 0;
    }

    public override void Call(Loc loc, VM vm, Stack stack, int arity)
    {
        var m = new OrderedMap<Value, Value>(arity / 2);

        for (var i = arity - 1; i >= 0; i -= 2)
        {
            var v = stack.Pop();
            var k = stack.Pop();
            m[k] = v;
        }

        stack.Push(Value.Make(this, m));
    }

    public override void Call(Loc loc, VM vm, Stack stack, Value target, int arity, int registerCount)
    {
        switch (arity)
        {
            case 1:
                {
                    var k = stack.Pop();
                    stack.Push(target.Cast(this)[k]);
                    break;
                }
            case 2:
                {
                    var v = stack.Pop();
                    target.Cast(this)[stack.Pop()] = v;
                    break;
                }
            default:
                throw new EvalError(loc, $"Wrong number of arguments: {arity}");

        }
    }

    public Order Compare(Value left, Value right)
    {
        var lm = left.Cast(this);
        var rm = right.Cast(this);
        var res = ComparableTrait.IntOrder(lm.Count.CompareTo(rm.Count));

        for (var i = 0; i < lm.Count && res != Order.EQ; i++)
        {
            var lv = lm.Items[i].Item1;
            var rv = rm.Items[i].Item1;

            if (lv.Type != rv.Type)
            {
                throw new Exception($"Type mismatch: {lv} {rv}");
            }

            if (lv.Type is ComparableTrait t && rv.Type is ComparableTrait)
            {
                res = t.Compare(lv, rv);
            }
            else
            {
                throw new Exception($"Not comparable: {lv} {rv}");
            }
        }

        return res;
    }

    /*
    public Iter CreateIter(Value target)
    {
        var t = target.Cast(this);
        return new MapItems(t);
    }*/

    public override void Dump(Value value, StringBuilder result)
    {
        result.Append('{');
        var i = 0;

        foreach (var v in value.Cast(this))
        {
            if (i > 0)
            {
                result.Append(' ');
            }

            v.Item1.Dump(result);
            result.Append(':');
            v.Item2.Dump(result);
            i++;
        }

        result.Append('}');
    }

    public override bool Equals(Value left, Value right)
    {
        var lv = left.Cast(this);
        var rv = right.Cast(this);

        if (lv.Count != rv.Count)
        {
            return false;
        }

        for (var i = 0; i < lv.Count; i++)
        {
            if (!lv.Items[i].Equals(rv.Items[i]))
            {
                return false;
            }
        }

        return true;
    }

    public override void Say(Value value, StringBuilder result)
    {
        result.Append('{');
        var i = 0;

        foreach (var v in value.Cast(this))
        {
            if (i > 0)
            {
                result.Append(' ');
            }

            v.Item1.Say(result);
            result.Append(':');
            v.Item2.Say(result);
            i++;
        }

        result.Append('}');
    }
}