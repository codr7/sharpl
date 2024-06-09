using System.Text;

namespace Sharpl;

public readonly struct Macro
{
    public delegate void BodyType(Loc loc, Macro target, VM vm, Env env, Form.Queue args);

    public readonly string[] Args;
    public readonly BodyType Body;
    public readonly string Name;

    public Macro(string name, string[] args, BodyType body)
    {
        Name = name;
        Args = args;
        Body = body;
    }

    public void Emit(Loc loc, VM vm, Env env, Form.Queue args)
    {
        Body(loc, this, vm, env, args);
    }

    public override string ToString()
    {
        var result = new StringBuilder();
        result.Append($"(Macro {Name} [");

        for (var i = 0; i < Args.Length; i++)
        {
            if (i > 0)
            {
                result.Append(' ');
            }

            result.Append(Args[i]);
        }

        result.Append("])");
        return result.ToString();
    }
}