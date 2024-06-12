using System.Runtime.Intrinsics.X86;
using System.Text;

namespace Sharpl;

public readonly struct UserMethod
{
    public readonly (string, int)[] Args;
    public readonly Loc Loc;
    public readonly string Name;
    public readonly int StartPC;

    public UserMethod(Loc loc, int startPC, string name, (string, int)[] args)
    {
        Loc = loc;
        StartPC = startPC;
        Name = name;
        Args = args;
    }

    public override string ToString()
    {
        var result = new StringBuilder();
        result.Append($"(Method {Name} [");

        for (var i = 0; i < Args.Length; i++)
        {
            if (i > 0)
            {
                result.Append(' ');
            }

            result.Append(Args[i].Item1);
        }

        result.Append("])");
        return result.ToString();
    }
}

