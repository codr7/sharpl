using System.Text;

namespace Sharpl.Types.Core;

public class TraitType(string name, AnyType[] parents) : ComparableType<UserTrait>(name, parents)
{
    public override void Dump(VM vm, Value value, StringBuilder result)
    {
        var t = value.Cast(this);
        result.Append($"(trait {t.Name} [{string.Join(' ', t.Parents.Where(pt => pt != t).Select(pt => pt.Name).ToArray())}])");
    }
}