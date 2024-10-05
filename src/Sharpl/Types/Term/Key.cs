using System.Text;

namespace Sharpl.Types.Term;

public class KeyType(string name, AnyType[] parents) : Type<ConsoleKeyInfo>(name, parents)
{
    public override void Dump(VM vm, Value value, StringBuilder result)
    {
        result.Append("(term/Key ");
        Say(vm, value, result);
        result.Append(')');
    }

    public override void Say(VM vm, Value value, StringBuilder result)
    {
        var ki = value.CastUnbox(this);
        if ((ki.Modifiers & ConsoleModifiers.Alt) != 0) { result.Append("Alt+"); }
        if ((ki.Modifiers & ConsoleModifiers.Control) != 0) { result.Append("Ctrl+"); }
        if ((ki.Modifiers & ConsoleModifiers.Shift) != 0) { result.Append("Shift+"); }
        result.Append(ki.Key.ToString());
    }
}
