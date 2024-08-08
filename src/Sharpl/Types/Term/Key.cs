namespace Sharpl.Types.Term;

using System.Text;

public class KeyType : Type<ConsoleKeyInfo>
{
    public KeyType(string name) : base(name) { }


    public override void Dump(Value value, StringBuilder result)
    {
        result.Append("(Key ");
        Say(value, result);
        result.Append(')');
    }

    public override void Say(Value value, StringBuilder result)
    {
        var ki = value.CastUnbox(this);
        
        if ((ki.Modifiers & ConsoleModifiers.Alt) != 0)
        {
            result.Append("Alt+");
        }

        if ((ki.Modifiers & ConsoleModifiers.Control) != 0)
        {
            result.Append("Ctrl+");
        }

        if ((ki.Modifiers & ConsoleModifiers.Shift) != 0)
        {
            result.Append("Shift+");
        }

        result.Append(ki.Key.ToString());
    }
}
