using System.Text;

namespace Sharpl.Types.Core;

public class CharType(string name, AnyType[] parents) :
    ComparableType<char>(name, parents), RangeTrait
{
    public override bool Bool(Value value) => value.CastUnbox(this) != 0;

    public Iter CreateRange(Value min, Value max, Value stride, Loc loc)
    {
        char minVal = (min.Type == Libs.Core.Nil) ? '\0' : min.CastUnbox(this, loc);
        char? maxVal = (max.Type == Libs.Core.Nil) ? null : max.CastUnbox(this, loc);
        int strideVal = (stride.Type == Libs.Core.Nil) ? ((maxVal is char mv && maxVal < minVal) ? -1 : 1) : stride.CastUnbox(Libs.Core.Int, loc);
        return new Iters.Core.CharRange(minVal, maxVal, strideVal);
    }

    public override void Dump(VM vm, Value value, StringBuilder result)
    {
        var c = value.CastUnbox(this);
        result.Append('\\');

        result.Append(c switch
        {
            '\n' => "\\n",
            '\r' => "\\r",
            _ => $"{c}"
        });
    }

    public override void Call(VM vm, Stack stack, Value target, int arity, int registerCount, bool eval, Loc loc)
    {
        var c = target.CastUnbox(this);
        var r = true;

        while (arity > 0)
        {
            if (stack.Pop().CastUnbox(this) != c) {  
                r = false; 
                break; 
            }

            arity--;
        }

        stack.Push(Libs.Core.Bit, r);
    }

    public override void Say(VM vm, Value value, StringBuilder result) =>
        result.Append(value.CastUnbox(this));
}