namespace Sharpl;

public readonly struct Method
{
    public delegate void BodyType(VM vm, Method target, int arity, Register result, Loc loc);

    public readonly string[] Args;
    public readonly BodyType Body;
    public readonly string Name;
    public readonly int MinArgCount;

    public Method(string name, string[] args, BodyType body)
    {
        Name = name;
        Args = args;
        MinArgCount = args.Count((a) => !a.EndsWith('?'));
        Body = body;

    }

    public void Call(VM vm, int arity, Register result, Loc loc)
    {
        if (arity < MinArgCount) { throw new EvalError($"Not enough arguments: {this}", loc); }
        Body(vm, this, arity, result, loc);
    }

    public override string ToString() =>
        $"(^{Name} [{string.Join(' ', Args)}])";
}