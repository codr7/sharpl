namespace Sharpl;

public static class StackExtensions
{
    public static string ToString(this List<Value> stack) =>
        $"[{string.Join(' ', stack)}]";
}