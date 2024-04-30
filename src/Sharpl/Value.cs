namespace Sharpl;

public readonly record struct Value(AnyType type, dynamic Data)
{
    public static Value Make<T>(Type<T> type, dynamic data)
    {
        return new Value(type, data);
    }
}