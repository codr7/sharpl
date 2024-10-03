using Sharpl.Iters.Core;
using System.Text;

namespace Sharpl.Types.Core;

using Sharpl.Libs;

public class ListType(string name, AnyType[] parents) : 
    Type<List<Value>>(name, parents), ComparableTrait, IterTrait, LengthTrait, StackTrait
{
    public override bool Bool(Value value) => value.Cast(this).Count != 0;

    public override void Call(VM vm, Stack stack, int arity, Loc loc)
    {
        stack.Reverse(arity);
        var vs = new List<Value>(arity);
        for (var i = arity - 1; i >= 0; i--) { vs.Add(stack.Pop()); }
        stack.Push(Value.Make(this, vs));
    }

    public override void Call(VM vm, Stack stack, Value target, int arity, int registerCount, bool eval, Loc loc)
    {
        switch (arity)
        {
            case 1:
                {
                    var t = target.Cast(this);
                    var iv = stack.Pop();

                    if (iv.Type == Core.Pair)
                    {
                        var p = iv.CastUnbox(Core.Pair);
                        var i = (p.Item1.Type == Core.Nil) ? 0 : p.Item1.CastUnbox(Core.Int, loc);
                        var n = (p.Item2.Type == Core.Nil) ? t.Count - 1 : p.Item2.CastUnbox(Core.Int, loc);
                        stack.Push(Core.List, t[i..(i + n)]);
                    }
                    else
                    {
                        var i = iv.CastUnbox(Core.Int);
                        stack.Push(t[i]);
                    }

                    break;
                }
            case 2:
                {
                    var v = stack.Pop();
                    target.Cast(this)[stack.Pop().CastUnbox(Core.Int)] = v;
                    break;
                }
            default:
                throw new EvalError($"Wrong number of arguments: {arity}", loc);

        }
    }

    public Order Compare(Value left, Value right)
    {
        var lvs = left.Cast(this);
        var rvs = right.Cast(this);
        var res = ComparableTrait.IntOrder(lvs.Count.CompareTo(rvs.Count));

        for (var i = 0; i < lvs.Count && res != Order.EQ; i++)
        {
            var lv = lvs[i];
            var rv = rvs[i];
            if (lv.Type != rv.Type) { throw new Exception($"Type mismatch: {lv} {rv}"); }
            if (lv.Type is ComparableTrait t && rv.Type is ComparableTrait) { res = t.Compare(lv, rv); }
            else { throw new Exception($"Not comparable: {lv} {rv}"); }
        }

        return res;
    }

    public Sharpl.Iter CreateIter(Value target, VM vm, Loc loc) =>
        new EnumeratorItems(target.Cast(this).GetEnumerator());

    public override Value Copy(Value value) =>
        Value.Make(this, new List<Value>(value.Cast(this).Select(it => it.Copy())));

    public override void Dump(Value value, VM vm, StringBuilder result)
    {
        result.Append("(List");

        foreach (var v in value.Cast(this))
        {
            result.Append(' ');
            v.Dump(vm, result);
        }

        result.Append(')');
    }

    public override bool Equals(Value left, Value right)
    {
        var lv = left.Cast(this);
        var rv = right.Cast(this);
        if (lv.Count != rv.Count) { return false; }

        for (var i = 0; i < lv.Count; i++)
        {
            if (!lv[i].Equals(rv[i])) { return false; }
        }

        return true;
    }

    public int Length(Value target) => target.Cast(this).Count;

    public Value Peek(Loc loc, VM vm, Value srcVal)
    {
        var src = srcVal.Cast(this);
        return (src.Count == 0) ? Value._ : src[^1];
    }

    public Value Pop(Loc loc, VM vm, Register src, Value srcVal)
    {
        var sv = srcVal.Cast(this);
        var n = sv.Count;
        if (n == 0) { return Value._; }
        n--;
        var v = sv[n];
        sv.RemoveAt(n);
        return v;
    }

    public void Push(Loc loc, VM vm, Register dst, Value dstVal, Value val) =>
        dstVal.Cast(this).Add(val);

    public override void Say(Value value, VM vm, StringBuilder result)
    {
        result.Append("(List");

        foreach (var v in value.Cast(this))
        {
            result.Append(' ');
            v.Say(vm, result);
        }

        result.Append(')');
    }

    public override string ToJson(Value value, Loc loc) =>
        $"[{string.Join(',', value.Cast(this).Select(it => it.ToJson(loc)).ToArray())}]";
}