
namespace Sharpl.Types.Core;

using System.Text;

public class StringType(string name, AnyType[] parents) : 
    ComparableType<string>(name, parents), IterTrait, LengthTrait, StackTrait
{
    static string Escape(string value) => value
        .Replace("\"", "\\\"")
        .Replace("\r", "\\r")
        .Replace("\n", "\\n");

    public override bool Bool(Value value) => value.Cast(this).Length != 0;

    public override void Call(VM vm, Stack stack, int arity, Loc loc)
    {
        stack.Reverse(arity);
        var res = new StringBuilder();

        while (arity > 0)
        {
            stack.Pop().Say(vm, res);
            arity--;
        }

        stack.Push(Value.Make(this, res.ToString()));
    }

    public override void Call(VM vm, Stack stack, Value target, int arity, int registerCount, bool eval, Loc loc)
    {
        switch (arity)
        {
            case 1:
                {
                    var iv = stack.Pop();
                    var t = target.Cast(this);

                    if (iv.Type == Libs.Core.Pair)
                    {
                        var p = iv.CastUnbox(Libs.Core.Pair);
                        var i = (p.Item1.Type == Libs.Core.Nil) ? 0 : p.Item1.CastUnbox(Libs.Core.Int, loc);
                        var n = (p.Item2.Type == Libs.Core.Nil) ? t.Length - i : p.Item2.CastUnbox(Libs.Core.Int, loc);
                        stack.Push(Libs.Core.String, t[i..(i + n)]);
                    }
                    else
                    {
                        var i = iv.CastUnbox(Libs.Core.Int, loc);
                        stack.Push(Libs.Core.Char, t[i]);
                    }

                    break;
                }
            case 2:
                {
                    var v = stack.Pop().CastUnbox(Libs.Core.Char, loc);
                    var s = target.Cast(this);
                    var cs = s.ToCharArray();
                    var i = stack.Pop().CastUnbox(Libs.Core.Int, loc);
                    cs[i] = v;
                    break;
                }
            default:
                throw new EvalError($"Wrong number of arguments: {arity}", loc);

        }
    }

    public Iter CreateIter(Value target, VM vm, Loc loc) =>
        new Iters.Core.EnumeratorItems(target.Cast(this).Select(c => Value.Make(Libs.Core.Char, c)).GetEnumerator());

    public override void Dump(VM vm, Value value, StringBuilder result) =>
      result.Append($"\"{Escape(value.Cast(this))}\"");

    public int Length(Value target) => target.Cast(this).Length;

    public Value Peek(Loc loc, VM vm, Value srcVal)
    {
        var src = srcVal.Cast(this);
        return (src.Length == 0) ? Value._ : Value.Make(Libs.Core.Char, src[^1]);
    }

    public Value Pop(Loc loc, VM vm, Register src, Value srcVal)
    {
        var sv = srcVal.Cast(this);
        if (sv.Length == 0) { return Value._; }
        var c = sv[^1];
        vm.Set(src, Value.Make(this, sv[0..^1]));
        return Value.Make(Libs.Core.Char, c);
    }

    public void Push(Loc loc, VM vm, Register dst, Value dstVal, Value val) =>
        vm.Set(dst, Value.Make(this, dstVal.Cast(this) + val.CastUnbox(Libs.Core.Char)));

    public override void Say(VM vm, Value value, StringBuilder result) => result.Append(value.Data);

    public override string ToJson(Value value, Loc loc) => $"\"{Escape(value.Cast(this))}\"";
}