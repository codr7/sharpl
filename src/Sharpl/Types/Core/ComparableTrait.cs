namespace Sharpl.Types.Core;

public interface ComparableTrait
{
    static Order IntOrder(int value)
    {
        return value switch
        {
            < 0 => Order.LT,
            > 0 => Order.GT,
            _ => Order.EQ
        };
    }

    static int OrderInt(Order value)
    {
        return value switch
        {
            Order.LT => -1,
            Order.GT => 1,
            _ => 0,
        };
    }

    Order Compare(Value left, Value right);
};