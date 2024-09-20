namespace Sharpl.Types.Core;

public interface RangeTrait
{
    Iter CreateRange(Loc loc, Value min, Value max, Value stride);
};