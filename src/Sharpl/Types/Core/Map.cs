namespace Sharpl.Types.Core;

using System.Text;

public class MapType : Type<OrderedMap<Value, Value>>, ComparableTrait, IterTrait, SeqTrait
{
    public MapType(string name) : base(name) { }

    public override bool Bool(Value value)
    {
        return value.Cast(this).Count != 0;
    }

    public override void Call(Loc loc, VM vm, Stack stack, int arity)
    {
        var m = new OrderedMap<Value, Value>();

        for (var i = 0; i < arity; i++)
        {
            var p = stack.Pop().CastUnbox(loc, Libs.Core.Pair);
            m[p.Item1] = p.Item2;
        }

        stack.Push(Value.Make(this, m));
    }

    public override void Call(Loc loc, VM vm, Stack stack, Value target, int arity, int registerCount)
    {
        switch (arity)
        {
            case 1:
                {
                    var m = target.Cast(this);
                    var k = stack.Pop();
                    
                    if (m.ContainsKey(k)) {
                        stack.Push(m[k]);
                    } else {
                        stack.Push(Value.Nil);
                    }

                    break;
                }
            case 2:
                {
                    var m = target.Cast(this);
                    var v = stack.Pop();
                    
                    if (v.Equals(Value.Nil)) {
                        m.Remove(stack.Pop());
                    } else {
                        m.Set(stack.Pop(), v);
                    }

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

    
    public Iter CreateIter(Value target)
    {
        var m = target.Cast(this);
        return new Iters.Core.MapItems(m.Items, 0, m.Count);
    }

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

    public int Length(Value target) {
        return target.Cast(this).Count;
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