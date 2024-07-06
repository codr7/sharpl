namespace Sharpl.Types.Core;

public interface ComparableTrait
{
    static Order IntOrder(int value)
    {
        if (value < 0)
        {
            return Order.LT;
        }

        if (value > 0)
        {
            return Order.GT;
        }

        return Order.EQ;
    }

    static int OrderInt(Order value)
    {
        switch (value)
        {
            case Order.LT:
                return -1;
            case Order.GT:
                return 1;
        }

        return 0;
    }

    Order Compare(Value left, Value right);
};