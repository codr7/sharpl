using Sharpl.Iters.Core;
using System.Text;

namespace Sharpl.Types.Core;

using Sharpl.Libs;

public class ArrayType : Type<Value[]>, ComparableTrait, IterTrait, LengthTrait, StackTrait
{
    public ArrayType(string name) : base(name) { }
    public override bool Bool(Value value) => value.Cast(this).Length != 0;

    public override void Call(Loc loc, VM vm, Stack stack, int arity)
    {
        var vs = new Value[arity];
        for (var i = arity - 1; i >= 0; i--) { vs[i] = stack.Pop(); }
        stack.Push(Value.Make(this, vs));
    }

    public override void Call(Loc loc, VM vm, Stack stack, Value target, int arity, int registerCount)
    {
        switch (arity)
        {
            case 1:
                {
                    var iv = stack.Pop();

                    if (iv.Type == Core.Pair)
                    {
                        var p = iv.CastUnbox(Core.Pair);
                        var i = p.Item1.CastUnbox(loc, Core.Int);
                        var n = p.Item2.CastUnbox(loc, Core.Int);
                        stack.Push(Core.Array, target.Cast(this)[i..(i + n)]);
                    }
                    else { stack.Push(target.Cast(this)[iv.CastUnbox(Core.Int)]); }
    
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
        var res = ComparableTrait.IntOrder(lvs.Length.CompareTo(rvs.Length));

        for (var i = 0; i < lvs.Length && res != Order.EQ; i++)
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
        new EnumeratorItems(((IEnumerable<Value>)target.Cast(this)).GetEnumerator());

    public override void Dump(Value value, StringBuilder result)
    {
        result.Append('[');
        var i = 0;

        foreach (var v in value.Cast(this))
        {
            if (i > 0) { result.Append(' '); }
            v.Dump(result);
            i++;
        }

        result.Append(']');
    }

    public override bool Equals(Value left, Value right)
    {
        var lv = left.Cast(this);
        var rv = right.Cast(this);
        if (lv.Length != rv.Length) { return false; }

        for (var i = 0; i < lv.Length; i++)
        {
            if (!lv[i].Equals(rv[i])) { return false; }
        }

        return true;
    }

    public int Length(Value target) => target.Cast(this).Length;

    public Value Peek(Loc loc, VM vm, Value srcVal) {
        var src = srcVal.Cast(this);
        return (src.Length == 0) ? Value.Nil : src[^1];
    }

    public Value Pop(Loc loc, VM vm, Register src, Value srcVal) {
        var sv = srcVal.Cast(this);
        if (sv.Length == 0) { return Value.Nil; }
        var v = sv[^1];
        vm.Set(src, Value.Make(this, sv[0..^1]));
        return v;
    }

    public void Push(Loc loc, VM vm, Register dst, Value dstVal, Value val)
    {
        var dv = dstVal.Cast(this);
        var i = dv.Length;
        Array.Resize(ref dv, i+1); 
        dv[i] = val;
        vm.Set(dst, Value.Make(this, dv));
    }

    public override void Say(Value value, StringBuilder result)
    {
        result.Append('[');
        var i = 0;

        foreach (var v in value.Cast(this))
        {
            if (i > 0) { result.Append(' '); }
            v.Say(result);
            i++;
        }

        result.Append(']');
    }
}