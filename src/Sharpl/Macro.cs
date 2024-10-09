namespace Sharpl;

public readonly struct Macro
{
    public delegate void BodyType(VM vm, Macro target, Form.Queue args, Register result, Loc loc);

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

    public void Emit(VM vm, Form.Queue args, Register result, Loc loc)
    {
        if (args.Count < MinArgCount) { throw new EmitError($"Not enough arguments: {this}", loc); }
        Body(vm, this, args, result, loc);
    }

    public override string ToString() =>
        $"({Name} [{string.Join(' ', Args)}])";
}