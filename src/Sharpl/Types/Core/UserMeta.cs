using System.Text;

namespace Sharpl.Types.Core;

public class UserMetaType(string name, AnyType[] parents) : ComparableType<UserType>(name, parents)
{
    public override void Dump(Value value, VM vm, StringBuilder result)
    {
        var t = value.Cast(this);
        result.Append($"(type {t.Name} [{string.Join(' ', t.Parents.Where(pt => pt != t).Select(pt => pt.Name).ToArray())}])");
    }
}