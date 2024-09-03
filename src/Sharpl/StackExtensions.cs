namespace Sharpl;

public static class StackExtensions
{
    public static string ToString(this List<Value> stack, VM vm) =>
        $"[{string.Join(' ', stack.Select(v => v.Dump(vm)))}]";
}