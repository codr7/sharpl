
namespace Sharpl.Types.Core;

using System.Runtime.InteropServices;
using System.Text;

public class StringType(string name) : ComparableType<string>(name), IterTrait, LengthTrait, StackTrait
{
    public override bool Bool(Value value) => value.Cast(this).Length != 0;

    public override void Call(Loc loc, VM vm, Stack stack, int arity)
    {
        stack.Reverse(arity);
        var res = new StringBuilder();

        while (arity > 0)
        {
            stack.Pop().Say(res);
            arity--;
        }

        stack.Push(Value.Make(this, res.ToString()));
    }

    public override void Call(Loc loc, VM vm, Stack stack, Value target, int arity, int registerCount)
    {
        switch (arity)
        {
            case 1:
                {
                    var iv = stack.Pop();
                    
                    if (iv.Type == Libs.Core.Pair) {
                        var p = iv.CastUnbox(Libs.Core.Pair);
                        var i = p.Item1.CastUnbox(loc, Libs.Core.Int);
                        var n = p.Item2.CastUnbox(loc, Libs.Core.Int);
                        stack.Push(Libs.Core.String, target.Cast(this)[i..(i+n)]);
                    } else {
                        var i = iv.CastUnbox(loc, Libs.Core.Int);
                        stack.Push(Libs.Core.Char, target.Cast(this)[i]);
                    }

                    break;
                }
            case 2:
                {
                    var v = stack.Pop().CastUnbox(loc, Libs.Core.Char);
                    var s = target.Cast(this);
                    var cs = s.ToCharArray();
                    var i = stack.Pop().CastUnbox(loc, Libs.Core.Int);
                    cs[i] = v;
                    break;
                }
            default:
                throw new EvalError(loc, $"Wrong number of arguments: {arity}");

        }
    }

    public Iter CreateIter(Value target) =>
        new Iters.Core.EnumeratorItems(target.Cast(this).Select(c => Value.Make(Libs.Core.Char, c)).GetEnumerator());

    public override void Dump(Value value, StringBuilder result)
    {
        result.Append('"');
        result.Append(value.Data);
        result.Append('"');
    }

    public int Length(Value target) => target.Cast(this).Length;

    public void Push(Loc loc, VM vm, Register dst, Value dstVal, Value val) =>
        vm.Set(dst, Value.Make(this, dstVal.Cast(this) + val.CastUnbox(Libs.Core.Char)));

    public override void Say(Value value, StringBuilder result) => result.Append(value.Data);
}
