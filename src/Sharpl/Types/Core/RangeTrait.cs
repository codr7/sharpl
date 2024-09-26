namespace Sharpl.Types.Core;

public interface RangeTrait
{
    Iter CreateRange(Value min, Value max, Value stride, Loc loc);
};