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
        if (args.Count < MinArgCount) { throw new EmitError(loc, $"Not enough arguments: {this}"); }
        Body(loc, this, vm, args);
    }

    public override string ToString() =>
        $"({Name} [{string.Join(' ', Args)}])";
}