using Sharpl.Iters.Core;
using System.Text;

namespace Sharpl.Types.Core;

using System.Reflection.PortableExecutable;
using Sharpl.Libs;

public class ListType : Type<List<Value>>, ComparableTrait, IterTrait, LengthTrait, StackTrait
{
    public ListType(string name) : base(name) { }
    public override bool Bool(Value value) => value.Cast(this).Count != 0;

    public override void Call(Loc loc, VM vm, Stack stack, int arity)
    {
        stack.Reverse(arity);
        var vs = new List<Value>(arity);
        for (var i = arity - 1; i >= 0; i--) { vs.Add(stack.Pop()); }
        stack.Push(Value.Make(this, vs));
    }

    public override void Call(Loc loc, VM vm, Stack stack, Value target, int arity, int registerCount)
    {
        switch (arity)
        {
            case 1:
                {
                    var t = target.Cast(this);
                    var iv = stack.Pop();
                    
                    if (iv.Type == Core.Pair) {
                        var p = iv.CastUnbox(Core.Pair);
                        stack.Push(Core.List, t[p.Item1.CastUnbox(loc, Core.Int)..(p.Item2.CastUnbox(loc, Core.Int)+1)]);
                    } else {
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
                throw new EvalError(loc, $"Wrong number of arguments: {arity}");

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

    public Sharpl.Iter CreateIter(Value target) =>
        new EnumeratorItems(target.Cast(this).GetEnumerator());

    public override void Dump(Value value, StringBuilder result)
    {
        result.Append("(List");

        foreach (var v in value.Cast(this))
        {
            result.Append(' ');
            v.Dump(result);
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

    public Value Peek(Loc loc, VM vm, Value srcVal) {
        var src = srcVal.Cast(this);
        return (src.Count == 0) ? Value.Nil : src[^1];
    }

    public Value Pop(Loc loc, VM vm, Register src, Value srcVal) {
        var sv = srcVal.Cast(this);
        var n = sv.Count;
        if (n == 0) { return Value.Nil; }
        n--;
        var v = sv[n];
        sv.RemoveAt(n);
        return v;
    }

    public void Push(Loc loc, VM vm, Register dst, Value dstVal, Value val) =>
        dstVal.Cast(this).Add(val);

    public override void Say(Value value, StringBuilder result)
    {
        result.Append("(List");

        foreach (var v in value.Cast(this))
        {
            result.Append(' ');
            v.Say(result);
        }

        result.Append(')');
    }
}