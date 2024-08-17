using System.Text;

namespace Sharpl.Types.Core;

public class CharType(string name) :
    ComparableType<char>(name),
    RangeTrait
{
    public override bool Bool(Value value) => value.CastUnbox(this) != 0;

    public Iter CreateRange(Loc loc, Value min, Value max, Value stride)
    {
        char minVal = (min.Type == Libs.Core.Nil) ? '\0' : min.CastUnbox(loc, this);
        char? maxVal = (max.Type == Libs.Core.Nil) ? null : max.CastUnbox(loc, this);
        int strideVal = (stride.Type == Libs.Core.Nil) ? ((maxVal is char mv && maxVal < minVal) ? -1 : 1) : stride.CastUnbox(loc, this);
        return new Iters.Core.CharRange(minVal, maxVal, strideVal);
    }

    public override void Dump(Value value, StringBuilder result) => 
        result.Append($"\\{value.CastUnbox(this)}");
}