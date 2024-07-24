using System.Text;

namespace Sharpl;

public readonly struct Macro
{
    public delegate void BodyType(Loc loc, Macro target, VM vm, Form.Queue args);

    public readonly string[] Args;
    public readonly BodyType Body;
    public readonly int MinArgCount;
    public readonly string Name;

    public Macro(string name, string[] args, BodyType body)
    {
        Name = name;
        Args = args;
        MinArgCount = args.Count((a) => !a.EndsWith('?'));
        Body = body;
    }

    public void Emit(Loc loc, VM vm, Form.Queue args)
    {
        if (args.Count < MinArgCount) {
            throw new EmitError(loc, $"Not enough arguments: {this}");
        }
        
        Body(loc, this, vm, args);
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