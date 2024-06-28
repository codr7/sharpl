namespace Sharpl.Types.Core;

using Sharpl.Iters.Core;
using Sharpl.Libs;
using System.Text;

public class PairType : Type<(Value, Value)>, ComparableTrait, IterTrait
{
    public PairType(string name) : base(name) { }

    public override void Call(Loc loc, VM vm, Stack stack, int arity)
    {
        if (arity != 2) {
            throw new EvalError(loc, "Wrong number of arguments");
        }

        var r = stack.Pop();
        var l = stack.Pop();
        stack.Push(Value.Make(Core.Pair, (l, r)));
    }

    public Order Compare(Value left, Value right)
    {
        var lp = left.Cast(this);
        var rp = right.Cast(this);
        
        if (lp.Item1.Type is ComparableTrait t) {
            var res = t.Compare(lp.Item1, rp.Item1);
            
            if (res != Order.EQ) {
                return res;
            }   

            return t.Compare(lp.Item2, rp.Item2);
        }

        return Order.EQ;
    }

    public override void Dump(Value value, StringBuilder result)
    {
        var p = value.Cast(this);
        p.Item1.Dump(result);
        result.Append(':');
        p.Item2.Dump(result);
    }

    public override bool Equals(Value left, Value right)
    {
        var lp = left.Cast(this);
        var rp = right.Cast(this);
        return lp.Item1.Equals(rp.Item1) && lp.Item2.Equals(rp.Item2);
    }

    public Iter Iter(Value target)
    {
        return new PairItems(target);
    }

    public override void Say(Value value, StringBuilder result)
    {
        var p = value.Cast(this);
        p.Item1.Say(result);
        result.Append(':');
        p.Item2.Say(result);
   }
}