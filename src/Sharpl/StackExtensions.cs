using System.Text;

namespace Sharpl;

public static class StackExtensions
{
    public static string ToString(this List<Value> stack)
    {
        var result = new StringBuilder();
        result.Append('[');
        var i = 0;

        foreach (var v in stack)
        {
            if (i > 0) { result.Append(' '); }
            v.Say(result);
            i++;
        }

        result.Append(']');
        return result.ToString();
    }
}