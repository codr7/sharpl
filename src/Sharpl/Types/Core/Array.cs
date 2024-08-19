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
                    var i = stack.Pop().CastUnbox(Core.Int);
                    stack.Push(target.Cast(this)[i]);
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

    public Value Push(Loc loc, Value dst, Value val) {
        var lst = new List<Value>();
        lst.AddRange(dst.Cast(this));
        lst.Add(val);
        return Value.Make(Core.List, lst);
    }

    public override void Say(Value value, StringBuilder result)
    {
        result.Append('[');
        var i = 0;

        foreach (var v in value.Cast(this))
        {
            if (i > 0)
            {
                result.Append(' ');
            }

            v.Say(result);
            i++;
        }

        result.Append(']');
    }
}