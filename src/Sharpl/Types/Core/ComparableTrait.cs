namespace Sharpl.Types.Core;

public interface ComparableTrait
{
    static Order IntOrder(int value) => (Order)Math.Sign(value);
    static int OrderInt(Order value) => (int)value;
    Order Compare(Value left, Value right);
};