using System.Text;

namespace Sharpl.Types.Core;

public class MapType(string name, AnyType[] parents) : 
    Type<OrderedMap<Value, Value>>(name, parents), ComparableTrait, IterTrait, LengthTrait
{
    public override bool Bool(Value value) => value.Cast(this).Count != 0;

    public override void Call(VM vm, Stack stack, int arity, Loc loc)
    {
        var m = new OrderedMap<Value, Value>();

        for (var i = 0; i < arity; i++)
        {
            var p = stack.Pop().CastUnbox(Libs.Core.Pair, loc);
            m[p.Item1] = p.Item2;
        }

        stack.Push(Value.Make(this, m));
    }

    public override void Call(VM vm, Stack stack, Value target, int arity, int registerCount, bool eval, Loc loc)
    {
        switch (arity)
        {
            case 1:
                {
                    var m = target.Cast(this);
                    var kv = stack.Pop();

                    if (kv.Type == Libs.Core.Pair)
                    {
                        var p = kv.CastUnbox(Libs.Core.Pair);
                        var i = (p.Item1.Type == Libs.Core.Nil) ? 0 : m.IndexOf(p.Item1);
                        if (i == -1) { throw new EvalError($"Key not found: {p.Item1}", loc); }
                        var j = (p.Item2.Type == Libs.Core.Nil) ? m.Count - 1 : m.IndexOf(p.Item2);
                        if (j == -1) { throw new EvalError($"Key not found: {p.Item2}", loc); }
                        stack.Push(Libs.Core.Map, new OrderedMap<Value, Value>(m.Items[i..(j + 1)]));
                    }
                    else { stack.Push(m.ContainsKey(kv) ? m[kv] : Value._); }

                    break;
                }
            case 2:
                {
                    var m = target.Cast(this);
                    var v = stack.Pop();
                    if (v.Equals(Value._)) { m.Remove(stack.Pop()); }
                    else { m.Set(stack.Pop(), v); }
                    break;
                }
            default:
                throw new EvalError($"Wrong number of arguments: {arity}", loc);

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
            if (lv.Type != rv.Type) { throw new Exception($"Type mismatch: {lv} {rv}"); }
            if (lv.Type is ComparableTrait t && rv.Type is ComparableTrait) { res = t.Compare(lv, rv); }
            else { throw new Exception($"Not comparable: {lv} {rv}"); }
        }

        return res;
    }


    public Iter CreateIter(Value target, VM vm, Loc loc) =>
        new Iters.Core.EnumeratorItems(target.Cast(this).Items.Select(v => Value.Make(Libs.Core.Pair, v)).GetEnumerator());

    public override Value Copy(Value value) =>
        Value.Make(this, new OrderedMap<Value, Value>(value.Cast(this).Items.Select(it => (it.Item1.Copy(), it.Item2.Copy())).ToArray()));

    public override void Dump(Value value, VM vm, StringBuilder result)
    {
        result.Append('{');
        var i = 0;

        foreach (var v in value.Cast(this))
        {
            if (i > 0) { result.Append(' '); }
            v.Item1.Dump(vm, result);
            result.Append(':');
            v.Item2.Dump(vm, result);
            i++;
        }

        result.Append('}');
    }

    public override bool Equals(Value left, Value right)
    {
        var lv = left.Cast(this);
        var rv = right.Cast(this);
        if (lv.Count != rv.Count) { return false; }

        for (var i = 0; i < lv.Count; i++)
        {
            if (!lv.Items[i].Equals(rv.Items[i])) { return false; }
        }

        return true;
    }

    public int Length(Value target) => target.Cast(this).Count;

    public override void Say(Value value, VM vm, StringBuilder result)
    {
        result.Append('{');
        var i = 0;

        foreach (var v in value.Cast(this))
        {
            if (i > 0) { result.Append(' '); }
            v.Item1.Say(vm, result);
            result.Append(':');
            v.Item2.Say(vm, result);
            i++;
        }

        result.Append('}');
    }

    public override string ToJson(Value value, Loc loc) =>
        $"{{{string.Join(',', value.Cast(this).Items.Select(it => $"{it.Item1.ToJson(loc)}:{it.Item2.ToJson(loc)}").ToArray())}}}";
}