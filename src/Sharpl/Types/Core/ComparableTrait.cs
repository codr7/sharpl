namespace Sharpl.Types.Core;

public interface ComparableTrait {
    static Order IntOrder(int value) {
        if (value < 0) {
            return Order.LT;
        }

        if (value > 0) {
            return Order.GT;
        }

        return Order.EQ;
    }

    Order Compare(Value left, Value right);
};