using Sharpl.Iters.Core;
using System.Text;

namespace Sharpl.Types.Core;

using Sharpl.Libs;

public class ArrayType(string name, AnyType[] parents) : 
    Type<Value[]>(name, parents), ComparableTrait, IterTrait, LengthTrait, StackTrait
{
    public override bool Bool(Value value) => value.Cast(this).Length != 0;

    public override void Call(VM vm, int arity, Register result, Loc loc)
    {
        var vs = new Value[arity];
        for (var i = 0; i < arity; i++) { vs[i] = vm.GetRegister(0, i); }
        vm.Set(result, Value.Make(this, vs));
    }

    public override void Call(VM vm, Value target, int arity, int registerCount, bool eval, Register result, Loc loc)
    {
        switch (arity)
        {
            case 1:
                {
                    var t = target.Cast(this);
                    var iv = vm.GetRegister(0, 0);

                    if (iv.Type == Core.Pair)
                    {
                        var p = iv.CastUnbox(Core.Pair);
                        var i = (p.Item1.Type == Core.Nil) ? 0 : p.Item1.CastUnbox(Core.Int, loc);
                        var n = (p.Item2.Type == Core.Nil) ? t.Length - 1 : p.Item2.CastUnbox(Core.Int, loc);
                        vm.Set(result, Value.Make(Core.Array, t[i..(i + n)]));
                    }
                    else vm.Set(result, t[iv.CastUnbox(Core.Int)]);

                    break;
                }
            case 2:
                {                    
                    target.Cast(this)[vm.GetRegister(0, 0).CastUnbox(Core.Int)] = vm.GetRegister(0, 1);
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

    public Sharpl.Iter CreateIter(Value target, VM vm, Loc loc) =>
        new EnumeratorItems(((IEnumerable<Value>)target.Cast(this)).GetEnumerator());

    public override Value Copy(Value value) =>
        Value.Make(this, value.Cast(this).Select(it => it.Copy()).ToArray());

    public override void Dump(VM vm, Value value, StringBuilder result)
    {
        result.Append('[');
        var i = 0;

        foreach (var v in value.Cast(this))
        {
            if (i > 0) { result.Append(' '); }
            v.Dump(vm, result);
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

    public Value Peek(Loc loc, VM vm, Value srcVal)
    {
        var src = srcVal.Cast(this);
        return (src.Length == 0) ? Value._ : src[^1];
    }

    public Value Pop(Loc loc, VM vm, Register src, Value srcVal)
    {
        var sv = srcVal.Cast(this);
        if (sv.Length == 0) { return Value._; }
        var v = sv[^1];
        vm.Set(src, Value.Make(this, sv[0..^1]));
        return v;
    }

    public void Push(Loc loc, VM vm, Register dst, Value dstVal, Value val)
    {
        var dv = dstVal.Cast(this);
        var i = dv.Length;
        Array.Resize(ref dv, i + 1);
        dv[i] = val;
        vm.Set(dst, Value.Make(this, dv));
    }

    public override void Say(VM vm, Value value, StringBuilder result)
    {
        result.Append('[');
        var i = 0;

        foreach (var v in value.Cast(this))
        {
            if (i > 0) { result.Append(' '); }
            v.Say(vm, result);
            i++;
        }

        result.Append(']');
    }

    public override string ToJson(Value value, Loc loc) =>
        $"[{string.Join(',', value.Cast(this).Select(it => it.ToJson(loc)).ToArray())}]";
}